namespace Blazr.Flux.Web.Data;

public class VisitorItemHandler : IItemHandler<Visitor>
{
    private readonly VisitDataProvider _visitDataProvider;

    public VisitorItemHandler(VisitDataProvider visitDataProvider)
    { 
        _visitDataProvider = visitDataProvider;
    }

    public ValueTask<Visitor> GetItemAsync(EntityUid uid)
    {
        return _visitDataProvider.GetVisitorAsync(uid.Value);
    }
}
