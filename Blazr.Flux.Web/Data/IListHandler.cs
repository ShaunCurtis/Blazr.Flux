namespace Blazr.Flux.Web.Data;

public interface IListHandler<TRecord>
     where TRecord : class
{
    public ValueTask<IEnumerable<TRecord>> GetItemsAsync(EntityUid uid);
}
