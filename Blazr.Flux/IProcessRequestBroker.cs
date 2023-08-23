/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public interface IProcessRequestBroker<TState>
    where TState : class, IOwsState
{
    public ValueTask<TState> DispatchAsync<TProcess>(TProcess process) where TProcess : class, IOwsProcessRequest;
}
