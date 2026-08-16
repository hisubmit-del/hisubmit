namespace HiSubmit.Application.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException():base("BadRequest Exceptions")
        {

        }
    }
}