namespace MoatGate.RequestHandling
{
    public class Query : Request, IQuery
    {
    }

    public class Query<T> : Request<T>, IQuery
    {
    }
}
