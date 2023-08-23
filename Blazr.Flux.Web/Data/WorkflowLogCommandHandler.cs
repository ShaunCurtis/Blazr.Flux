/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web.Data;

public class WorkflowLogCommandHandler : ICommandHandler<WorkflowLog>
{
    private readonly VisitDataProvider _visitDataProvider;

    public WorkflowLogCommandHandler(VisitDataProvider visitDataProvider)
    {
        _visitDataProvider = visitDataProvider;
    }

    public async ValueTask<CommandResult> ExecuteAsync(WorkflowLog workflowLog)
    {
        await _visitDataProvider.WorkflowLogCommand(workflowLog);
        return new(Successful: true);
    }
}
