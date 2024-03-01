using System.Net;

namespace Infrastructure.Middleware.Exceptions.Common
{

    public class ConflictException : CustomException
    {
        public ConflictException(string message)
            : base(message, null, HttpStatusCode.Conflict)
        {
        }
    }
}