namespace Application.Exceptions;

public class ErrorException : Exception
{
    public ErrorException(string message) : base(message) {}
}