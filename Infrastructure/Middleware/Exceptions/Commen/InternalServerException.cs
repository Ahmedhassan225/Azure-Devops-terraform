using System.Collections.Generic;
using System.Net;

namespace Infrastructure.Middleware.Exceptions.Common
{
    public class InternalServerException : CustomException
    {
        public InternalServerException(string message, List<string>? errors = default)
            : base(message, errors, HttpStatusCode.InternalServerError)
        {
        }
    }
}