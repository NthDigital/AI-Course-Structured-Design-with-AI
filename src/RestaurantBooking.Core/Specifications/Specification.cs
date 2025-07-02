using System.Linq.Expressions;

namespace RestaurantBooking.Core.Specifications;

public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public Func<T, bool> ToFunc()
    {
        return ToExpression().Compile();
    }

    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToFunc();
        return predicate(entity);
    }

    public Specification<T> And(Specification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }

    public Specification<T> Or(Specification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}
