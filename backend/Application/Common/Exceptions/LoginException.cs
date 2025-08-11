namespace Application.Common.Exceptions;

public class LoginException : Exception
{
    public LoginException(string name) : base(name) { }
    public LoginException(string name, Exception inner): base(name, inner) { }
}