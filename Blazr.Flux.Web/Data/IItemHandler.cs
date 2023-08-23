namespace Blazr.Flux.Web.Data;

public interface IItemHandler<TRecord>
     where TRecord : class, IOwsState
{
    public ValueTask<TRecord> GetItemAsync(EntityUid uid);
}
