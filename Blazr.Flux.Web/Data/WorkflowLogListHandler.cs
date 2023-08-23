namespace Blazr.Flux.Web.Data;

public class WorkflowLogListHandler : IListHandler<WorkflowLog>
{
    private readonly VisitDataProvider _visitDataProvider;

    public WorkflowLogListHandler(VisitDataProvider visitDataProvider)
    { 
        _visitDataProvider = visitDataProvider;
    }

    public ValueTask<IEnumerable<WorkflowLog>> GetItemsAsync(EntityUid uid)
    {
        return _visitDataProvider.GetLogsAsync(uid);
    }
}
