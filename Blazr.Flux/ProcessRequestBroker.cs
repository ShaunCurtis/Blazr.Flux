/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public class ProcessRequestBroker<TState> : IProcessRequestBroker<TState>  
    where TState : class, IOwsState
{
    private IProcessRequestResolver<TState> _processHandlerResolver;

    public ProcessRequestBroker(IProcessRequestResolver<TState> processHandlerResolver)
    {
        _processHandlerResolver = processHandlerResolver;
    }

    public async ValueTask<TState> DispatchAsync<TProcess>(TProcess process) where TProcess : class, IOwsProcessRequest
    {
        var handler = _processHandlerResolver.GetHandler(process);

        return await handler.DispatchAsync(process);
    }
}
