namespace HeistPlanner.Services
{
    //------ ref: user-defined exceptions for the error and exception handling from rubric : https://learn.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions

    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
        }
    }
}