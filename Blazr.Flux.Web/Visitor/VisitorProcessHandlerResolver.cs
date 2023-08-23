/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web;

public class VisitorProcessHandlerResolver : IProcessRequestResolver<Visitor>
{
    private readonly IServiceProvider _serviceProvider;
    private IProcessHandler<Visitor> _processHandler;

    public VisitorProcessHandlerResolver(IServiceProvider serviceProvider, IProcessHandler<Visitor> processHandler)
    { 
        _serviceProvider = serviceProvider;
        _processHandler = processHandler;
    }

    public IProcessHandler<Visitor> GetHandler<TProcess>(TProcess process) where TProcess : class, IOwsProcessRequest
    {
        IProcessHandler<Visitor>? handler = null;

        if (process is OnSiteProcess)
            handler = _serviceProvider.GetService<IProcessHandler<Visitor, OnSiteProcess>>() as IProcessHandler<Visitor>;

        if (process is OffSiteProcess)
            handler = _serviceProvider.GetService<IProcessHandler<Visitor, OffSiteProcess>>() as IProcessHandler<Visitor>;

        if (process is CompletedProcess)
            handler = _serviceProvider.GetService<IProcessHandler<Visitor, CompletedProcess>>() as IProcessHandler<Visitor>;

        return handler ?? _processHandler;
    }
}
