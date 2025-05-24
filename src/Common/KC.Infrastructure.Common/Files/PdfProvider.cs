using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KC.Domain.Common.Extensions;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.AcroForms;
using PdfSharpCore.Pdf.IO;

namespace KC.Infrastructure.Common.Files
{
    /// <summary>
    /// PDF provider class that implements IPdfProvider
    /// </summary>
    public class PdfProvider : IPdfProvider
    {
        private readonly ILogger<PdfProvider> _logger;

        #region Constructors

        public PdfProvider(ILogger<PdfProvider> logger)
        {
            _logger = logger;
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // dispose resources
        }

        #endregion

        #region IPdfProvider Methods

        public async Task<byte[]> MergeAsync(IList<Domain.Common.Files.File> files, CancellationToken cancellationToken = default)
        {
            // create the output document (need to use MergeBase.pdf which contains an AcroForm)
            var assembly = GetType().Assembly;
            var resourceName = $"{assembly.GetName().Name}.Files.MergeBase.pdf";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            var outputDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
            outputDoc.Pages.RemoveAt(0);
            if (outputDoc.AcroForm.Elements.ContainsKey("/NeedAppearances"))
            {
                outputDoc.AcroForm.Elements["/NeedAppearances"] = new PdfBoolean(true);
            }
            else
            {
                outputDoc.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));
            }

            foreach (var file in files)
            {
                if (file.Content?.Length > 0)
                {
                    try
                    {
                        using var ms = new MemoryStream();
                        await ms.WriteAsync(file.Content, cancellationToken);
                        var inputDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Import);
                        foreach (PdfPage page in inputDoc.Pages)
                        {
                            outputDoc.AddPage(page);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error merging PDF file: {FileName}.", file.Name);
                    }
                }
            }

            using var oms = new MemoryStream();
            outputDoc.Save(oms);

            return oms.ToArray();
        }

        public byte[] SetFields(byte[] pdfContent, Dictionary<string, object?> fields)
        {
            if (pdfContent.Length == 0 || fields.Count == 0)
            {
                return pdfContent;
            }

            using var document = PdfReader.Open(new MemoryStream(pdfContent), PdfDocumentOpenMode.Modify);
            var form = document.AcroForm;
            if (form.Elements.ContainsKey("/NeedAppearances"))
            {
                form.Elements["/NeedAppearances"] = new PdfBoolean(true);
            }
            else
            {
                form.Elements.Add("/NeedAppearances", new PdfBoolean(true));
            }
            foreach (var fieldName in form.Fields.Names)
            {
                var field = form.Fields[fieldName];
                var isReadOnly = field.ReadOnly;
                field.ReadOnly = false;
                if (field is PdfTextField textField)
                {
                    var name = field.Name;
                    string? format = null;
                    if (name.Contains(':'))
                    {
                        var parts = name.Split(':', StringSplitOptions.TrimEntries);
                        name = parts[0];
                        format = parts[1];
                    }
                    if (fields.TryGetValue(name, out var value) && value is not null)
                    {
                        if (value.ToString()?.Equals("#hidden#", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            isReadOnly = true;
                        }
                        else
                        {
                            if (format is not null && value is IFormattable val)
                            {
                                textField.Value = new PdfString(val.ToString(format, null));
                            }
                            else
                            {
                                textField.Value = new PdfString(value.ToString());
                            }
                        }
                    }
                }
                else if (field is PdfCheckBoxField checkBoxField && fields.ContainsKey(field.Name))
                {
                    checkBoxField.Checked = fields.GetValueOrDefault<bool>(field.Name);
                }
                else if (field is PdfRadioButtonField)
                {
                    var value = fields.GetValueOrDefault<string>(field.Name);
                    if (value is not null)
                    {
                        field.Value = new PdfName($"/{value}");
                    }
                }
                field.ReadOnly = isReadOnly;
            }

            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }
        #endregion
    }
}
