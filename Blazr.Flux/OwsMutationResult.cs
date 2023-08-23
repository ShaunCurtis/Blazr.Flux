/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public record OwsMutationResult<TState> : IOwsResult<TState>
    where TState : class, IOwsState
{
    public TState? State { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public static OwsMutationResult<TState> Success(TState state, string? message = null)
        => new OwsMutationResult<TState>() { Successful = true, Message = message, State = state };

    public static OwsMutationResult<TState> Failure(string message, TState? state = null)
        => new OwsMutationResult<TState>() { Successful = false, Message = message, State = state };
}