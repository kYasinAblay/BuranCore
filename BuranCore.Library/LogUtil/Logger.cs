using System;

namespace Buran.Core.Library.LogUtil
{
    public class Logger
    {
        public static string GetErrorMessage(Exception ex, bool trace = false)
        {
            string errorMessage;
            while (true)
            {
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                else
                {
                    errorMessage = ex.Message.Replace("\"", "'");
                    break;
                }
            }
            if (trace)
            {
                errorMessage += ex.StackTrace;
            }

            return errorMessage;
        }
    }
}
