using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KC.Utils.Common
{
    public static class XmlUtil
    {
        public static T? TryXmlParseTo<T>(this string s)
        {
            T? val = default;

            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    XDocument xd = XDocument.Parse(s);
                    xd.Descendants()
                        .Where(e => e.Attributes().All(a => a.IsNamespaceDeclaration || string.IsNullOrWhiteSpace(a.Value))
                                && string.IsNullOrWhiteSpace(e.Value)
                                && e.Descendants().SelectMany(c => c.Attributes()).All(ca => ca.IsNamespaceDeclaration || string.IsNullOrWhiteSpace(ca.Value)))
                        .Remove();
                    if (xd.Root is not null)
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        using XmlReader reader = xd.Root.CreateReader();
                        val = (T?)serializer.Deserialize(reader);
                    }
                }
                catch (System.Exception ex)
                {
                    // ignore, default value will be returned
                    System.Console.Write(ex);
                }
            }

            return val;
        }

        public static T? TryXmlFragmentParseTo<T>(this string s, string fragmentName)
        {
            T? val = default;

            if (!string.IsNullOrEmpty(s))
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment
                };
                using var reader = XmlReader.Create(new StringReader(s), settings);
                while (!reader.EOF)
                {
                    if (reader.Name != fragmentName)
                    {
                        reader.ReadToFollowing(fragmentName);
                    }
                    if (!reader.EOF)
                    {
                        var serializer = new XmlSerializer(typeof(T));
                        return (T?)serializer.Deserialize(reader);
                    }
                }
            }

            return val;
        }

        public static string? ToSoap12Xml<T>(this T o)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("soap12", "http://www.w3.org/2003/05/soap-envelope");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            Stream ms = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true
            };

            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                serializer.Serialize(writer, o, ns);
            }

            ms.Position = 0;
            using StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static T? FromSoapXml<T>(this string? xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using var sr = new StringReader(xml);
                using var reader = XmlReader.Create(sr);
                return (T?)Convert.ChangeType(serializer.Deserialize(reader), typeof(T));
            }
            return default;
        }

        public static string SerializeToXml<T>(this T obj, bool indent = false, bool includeNamespaces = true)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using var writer = new StringWriter();
            using XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            if (indent)
            {
                xmlWriter.Formatting = Formatting.Indented;
            }
            if (includeNamespaces)
            {
                serializer.Serialize(xmlWriter, obj);
            }
            else
            {
                var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                serializer.Serialize(xmlWriter, obj, ns);
            }
            return writer.ToString();
        }
    }
}
