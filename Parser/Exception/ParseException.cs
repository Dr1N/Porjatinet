namespace Parser.Exception
{
    public class ParseException : System.Exception
    {
        public ParseException(string message) : base(message)
        {
        }

        public ParseException() : base()
        {
        }

        public ParseException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}