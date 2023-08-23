/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

/// <summary>
/// Delegate Declaration for a mutation
/// It defines a function that receives a TState object
/// And returns a new TState object with the mutation applied
/// </summary>
/// <typeparam name="TState"></typeparam>
/// <param name="state"></param>
/// <returns></returns>
public delegate Task<OwsMutationResult<TState>> OwsStateMutationDelegate<TState>( OwsMutationRequest<TState> request) where TState : class, IOwsState;