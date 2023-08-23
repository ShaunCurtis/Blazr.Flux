/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public record OwsMutationRequest<TState>(TState State, CancellationToken CancellationToken = new()) 
    where TState : class, IOwsState;
