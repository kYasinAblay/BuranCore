using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Buran.Core.Library.Utils;

namespace Buran.Core.MvcLibrary.ModelBinders
{
    public class DateTimeModelBinder : IModelBinder
    {
        private string format;
        public DateTimeModelBinder(string format)
        {
            this.format = format;
        }
        private DateTime? GetA(ModelBindingContext bindingContext, string key)
        {
            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
                key = key.IsEmpty() ? bindingContext.ModelName : bindingContext.ModelName + "." + key;
            if (key.IsEmpty())
                return null;

            var value = bindingContext.ValueProvider.GetValue(key);
            bindingContext.ModelState.SetModelValue(key, value);
            DateTime? retVal = null;
            try
            {
                var cu = new CultureInfo(format);
                retVal = DateTime.Parse(value.ToString(), cu);
            }
            catch
            {
            }
            return retVal;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext");
            var dateTimeAttempt = GetA(bindingContext, "");
            if (dateTimeAttempt != null)
            {
                bindingContext.Result = ModelBindingResult.Success(dateTimeAttempt.Value);
                return Task.CompletedTask;
            }

            var dateAttempt = GetA(bindingContext, "Date");
            var timeAttempt = GetA(bindingContext, "TimeOfDay");
            if (dateAttempt != null && timeAttempt != null)
            {
                bindingContext.Result = ModelBindingResult.Success(new DateTime(dateAttempt.Value.Year,
                    dateAttempt.Value.Month,
                    dateAttempt.Value.Day,
                    timeAttempt.Value.Hour,
                    timeAttempt.Value.Minute,
                    timeAttempt.Value.Second));
            }
            else
                bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }
    }
}