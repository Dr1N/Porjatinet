namespace Common.Exception
{
    public class RepositoryException : System.Exception
    {
        public RepositoryException(string message) : base(message)
        {
        }

        public RepositoryException()
        {
        }

        public RepositoryException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}