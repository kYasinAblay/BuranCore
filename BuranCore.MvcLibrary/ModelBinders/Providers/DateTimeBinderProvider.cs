using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Buran.Core.MvcLibrary.ModelBinders.Providers
{
    public class DateTimeBinderProvider : IModelBinderProvider
    {
        private string format;
        public DateTimeBinderProvider(string format)
        {
            this.format = format;
        }
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Metadata.UnderlyingOrModelType == typeof(DateTime))
                return new DateTimeModelBinder(format);
            return null;
        }
    }
}
