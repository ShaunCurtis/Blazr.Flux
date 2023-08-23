/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web.Data;

public class OnSiteWorkflowHandler : VisitorWorkflowHandler, IProcessHandler<Visitor, OnSiteProcess>
{
    public OnSiteWorkflowHandler(OwsEntityStore<Visitor> store, ICommandHandler<Visitor> visitorCommandHandler, ICommandHandler<WorkflowLog> workflowCommandHandler) 
        : base(store, visitorCommandHandler, workflowCommandHandler) { }

    public async ValueTask<Visitor> DispatchAsync(OnSiteProcess process)
        => await base.DispatchAsync(process);

    protected override Task<Visitor> ExecuteMutationAsync(Visitor state)
    {
        var newEntityState = state.EntityState with { StateCode = this.Process?.FinalState ?? state.EntityState.StateCode };

        var newState = state with
        {
            EntityState = newEntityState,
            OnSite = this.Process?.TimeDate
        };

        return Task.FromResult(newState);
    }
}

public class OffSiteWorkflowHandler : VisitorWorkflowHandler, IProcessHandler<Visitor, OffSiteProcess>
{
    public OffSiteWorkflowHandler(OwsEntityStore<Visitor> store, ICommandHandler<Visitor> visitorCommandHandler, ICommandHandler<WorkflowLog> workflowCommandHandler)
        : base(store, visitorCommandHandler, workflowCommandHandler) { }

    public async ValueTask<Visitor> DispatchAsync(OffSiteProcess process)
        => await base.DispatchAsync(process);

    protected override Task<Visitor> ExecuteMutationAsync(Visitor state)
    {
        var newEntityState = state.EntityState with { StateCode = this.Process?.FinalState ?? state.EntityState.StateCode };

        var newState = state with
        {
            EntityState = newEntityState,
            OffSite = this.Process?.TimeDate
        };

        return Task.FromResult(newState);
    }
}

public class CompletedWorkflowHandler : VisitorWorkflowHandler, IProcessHandler<Visitor, CompletedProcess>
{
    public CompletedWorkflowHandler(OwsEntityStore<Visitor> store, ICommandHandler<Visitor> visitorCommandHandler, ICommandHandler<WorkflowLog> workflowCommandHandler)
        : base(store, visitorCommandHandler, workflowCommandHandler) { }

    public async ValueTask<Visitor> DispatchAsync(CompletedProcess process)
        => await base.DispatchAsync(process);

    protected override Task<Visitor> ExecuteMutationAsync(Visitor state)
    {
        // If the visitor is on site we need to set the off site time
        // If they are already off site (and not returning) then we use the exiitng time

        var offsiteTime = state.OffSite;

        if (state.EntityState.StateCode == VisitorStateCodes.OnSite)
            offsiteTime = this.Process?.TimeDate;

        var newEntityState = state.EntityState with { StateCode = this.Process?.FinalState ?? state.EntityState.StateCode };

        var newState = state with
        {
            EntityState = newEntityState,
            OffSite = this.Process?.TimeDate
        };

        return Task.FromResult(newState);
    }
}

