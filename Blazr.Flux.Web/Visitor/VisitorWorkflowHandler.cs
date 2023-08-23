/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public class VisitorWorkflowHandler : IProcessHandler<Visitor>
{
    private readonly OwsEntityStore<Visitor> _store;
    private readonly ICommandHandler<Visitor> _visitorCommandHandler;
    private readonly ICommandHandler<WorkflowLog> _workflowLogCommandHandler;

    protected IOwsProcessRequest? Process;

    public VisitorWorkflowHandler(OwsEntityStore<Visitor> store, ICommandHandler<Visitor> visitorCommandHandler, ICommandHandler<WorkflowLog> workflowCommandHandler)
    {
        _store = store;
        _visitorCommandHandler = visitorCommandHandler;
        _workflowLogCommandHandler = workflowCommandHandler;
    }

    public async ValueTask<Visitor> DispatchAsync<TProcess>(TProcess process) where TProcess : class, IOwsProcessRequest
    {
        this.Process = process;
        return await _store.DispatchAsync(process.StateUid, this.ExecuteAsync);
    }

    public async ValueTask<Visitor> DispatchAsync(IOwsProcessRequest process)
    {
        this.Process = process;
        return await _store.DispatchAsync(process.StateUid, this.ExecuteAsync);
    }

    protected virtual Task<Visitor> ExecuteMutationAsync(Visitor state)
    {
        if (this.Process is null)
            return Task.FromResult(state);

        var newState = state with { EntityState = state.EntityState.Mutate(this.Process!.FinalState) };

        return Task.FromResult(newState);
    }

    protected async virtual Task<bool> ExecutePersistAsync(Visitor previousState, Visitor newState)
    {
        if (this.Process is null)
            return false;

        // Create the log
        var log = new WorkflowLog
        {
            WorkflowLogUid = new(Guid.NewGuid()),
            StateUid = previousState.StateUid,
            CreatorUid = new(Guid.Empty),
            InitialState = previousState.EntityState.StateCode,
            FinalState = this.Process.FinalState,
            StateCode = StateCodes.New,
            Log = $"Changed State from {previousState.EntityState.StateCode.Title} to {this.Process.FinalState.Title}",
            TimeStamp = DateTime.UtcNow
        };

        //Normally a DB Transaction
        {
            await _visitorCommandHandler.ExecuteAsync(newState);

            await _workflowLogCommandHandler.ExecuteAsync(log);
        }

        return true;
    }

    private async Task<OwsMutationResult<Visitor>> ExecuteAsync(OwsMutationRequest<Visitor> request)
    {
        if (this.Process is null)
            return OwsMutationResult<Visitor>.Failure("No Process defined", request.State with { });

        // TODO - build in error code if things go wrong
        var newState = await this.ExecuteMutationAsync(request.State);

        await this.ExecutePersistAsync(request.State, newState);
 
        return OwsMutationResult<Visitor>.Success(newState);
    }
}
