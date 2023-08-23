/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web;

public static class VisitorWorkflowServices
{
    public static void RegisterVisitorWorkflowServices(this IServiceCollection services)
    {
        // Data Store Emulator
        services.AddSingleton<VisitDataProvider>();

        // Data Pipeline Handlers
        services.AddScoped<IItemHandler<Visitor>, VisitorItemHandler>();
        services.AddScoped<ICommandHandler<Visitor>, VisitorCommandHandler>();

        services.AddScoped<IListHandler<WorkflowLog>, WorkflowLogListHandler>();
        services.AddScoped<ICommandHandler<WorkflowLog>, WorkflowLogCommandHandler>();

        // Ows Store services
        services.AddScoped<OwsEntityStore<Visitor>>();

        // Workflow Services
        services.AddScoped<IProcessRequestBroker<Visitor>, ProcessRequestBroker<Visitor>>();
        services.AddScoped<IProcessRequestResolver<Visitor>, VisitorProcessHandlerResolver>();

        services.AddTransient<IProcessHandler<Visitor>, VisitorWorkflowHandler>();
        services.AddTransient<IProcessHandler<Visitor, OnSiteProcess>, OnSiteWorkflowHandler>();
        services.AddTransient<IProcessHandler<Visitor, OffSiteProcess>, OffSiteWorkflowHandler>();
        services.AddTransient<IProcessHandler<Visitor, CompletedProcess>, CompletedWorkflowHandler>();

        // Presentation Services
        services.AddScoped<VisitorWorkflowManager>();

    }
}
