/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public record OwsProcessResult<TState> : IOwsResult<TState>
    where TState : class, IOwsState
{
    public TState? State { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static OwsProcessResult<TState> Success(TState state, string? message = null)
        => new OwsProcessResult<TState>() { Successful = true, Message = message, State = state };

    public static OwsProcessResult<TState> Failure(string message, TState? state = null)
        => new OwsProcessResult<TState>() { Successful = false, Message = message, State = state };
}