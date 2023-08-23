/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public interface IProcessRequestResolver<TState>
    where TState : class, IOwsState
{
    public IProcessHandler<TState> GetHandler<TProcess>(TProcess process) where TProcess : class, IOwsProcessRequest;
}
