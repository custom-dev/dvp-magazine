using System;
using System.Collections.Generic;
using System.Text;

namespace Developpez.MagazineTool
{
    public static class ExceptionExtension
    {
        public  static string FullTrace(this Exception ex)
        {
            Exception currentEx = ex;
            StringBuilder builder = new StringBuilder();

            while(currentEx != null)
            {
                builder.AppendLine(ex.GetType().ToString());
                builder.AppendLine(ex.Message);
                builder.AppendLine(ex.StackTrace);
                builder.AppendLine();

                currentEx = currentEx.InnerException;
            }

            return builder.ToString();
        }
    }
}
