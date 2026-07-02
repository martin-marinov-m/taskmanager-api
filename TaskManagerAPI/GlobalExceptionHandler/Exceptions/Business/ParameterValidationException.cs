namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business
{
    public class ParameterValidationException : TaskManagerException
    {
        public string ParameterName { get; }
        public string ParameterValue { get; }

        public override string Title => "Validation failed";
        public override int StatusCode => StatusCodes.Status400BadRequest;

        public ParameterValidationException(string message, string parameterName, string parameterValue) : base(message)
        {
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }
    }
}