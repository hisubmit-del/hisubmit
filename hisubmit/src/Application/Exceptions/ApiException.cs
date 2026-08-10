using System;
using System.Globalization;

namespace HiSubmit.Application.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() : base()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }

    public class DontPermissionException : Exception
    {
        public DontPermissionException() : 
            base("You do not have permission to access this resource")
        {
        }

        public DontPermissionException(string message) : base(message)
        {
        }

        public DontPermissionException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}