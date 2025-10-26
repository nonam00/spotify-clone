namespace Application.Shared.Exceptions;

public class OwnershipException : Exception
{
    public OwnershipException(string name) : base(name) { }
    public OwnershipException(string name, Exception inner): base(name, inner) { }
}