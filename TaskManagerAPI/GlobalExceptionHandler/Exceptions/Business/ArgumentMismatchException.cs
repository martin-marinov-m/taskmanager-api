namespace TaskManagerAPI.GlobalExceptionHandler.Exceptions.Business
{
    public class ArgumentMismatchException : TaskManagerException
    {
        public string ExpectedName { get; }

        public string ExpectedValue { get; }

        public string ActualName { get; }

        public string ActualValue { get; }

        public override string Title => "Arguments mismatch";
        public override int StatusCode => StatusCodes.Status400BadRequest;

        public ArgumentMismatchException(string expectedName, string expectedValue, string actualName, string actualValue) : base($"{expectedName} does not match {actualName}.")
        {
            ExpectedName = expectedName;
            ExpectedValue = expectedValue;
            ActualName = actualName;
            ActualValue = actualValue;
        }
    }
}