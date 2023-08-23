/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public interface IOwsResult<TState>
    where TState : class, IOwsState
{
    public TState? State { get; }
    public bool Successful { get; }
    public string? Message { get; }
}