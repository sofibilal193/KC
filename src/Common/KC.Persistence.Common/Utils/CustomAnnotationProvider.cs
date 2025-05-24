using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;

namespace KC.Persistence.Common.Utils
{
    public class CustomAnnotationProvider : SqlServerAnnotationProvider
    {
        public CustomAnnotationProvider(RelationalAnnotationProviderDependencies dependencies) : base(dependencies)
        {
        }

        public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
        {
            var annotations = base.For(column, designTime);
            var property = column.PropertyMappings[0]?.Property;
            var annotation = property?.FindAnnotation(CustomAnnotations.SensitivityClassification);
            if (annotation is not null)
            {
                annotations = annotations.Concat(new[] { annotation });
            }
            annotation = property?.FindAnnotation(CustomAnnotations.DynamicDataMask);
            if (annotation is not null)
            {
                annotations = annotations.Concat(new[] { annotation });
            }
            return annotations;
        }
    }
}
