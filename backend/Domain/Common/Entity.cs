namespace Domain.Common;

public abstract class Entity
{
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return GetType() == other.GetType() && 
               GetType().GetProperties().All(p => Equals(p.GetValue(this), p.GetValue(other)));
    }

    public override int GetHashCode()
    {
        return GetType().GetProperties()
            .Select(p => p.GetValue(this))
            .Where(v => v != null)
            .Aggregate(0, (hash, value) => hash ^ value!.GetHashCode());
    }
}

public abstract class Entity<TId> : Entity
{
    public TId Id { get; protected init; } = default!;

    protected Entity() { }
    protected Entity(TId id)
    {
        Id = id;
    }
}