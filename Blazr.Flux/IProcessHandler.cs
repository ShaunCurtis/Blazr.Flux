/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public interface IProcessHandler<TState>
    where TState : class, IOwsState
{
    public ValueTask<TState> DispatchAsync(IOwsProcessRequest process);
}

public interface IProcessHandler<TState, TProcess>
    where TState : class, IOwsState
    where TProcess : class, IOwsProcessRequest
{
    public ValueTask<TState> DispatchAsync(TProcess process);
}
