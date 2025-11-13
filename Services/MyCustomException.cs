namespace Services
{
    public class MyCustomException : Exception
    {
        public MyCustomException(string message) : base(message)
        {
        }
    }
}
