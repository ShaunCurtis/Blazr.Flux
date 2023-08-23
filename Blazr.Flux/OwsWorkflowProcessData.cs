/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

/// <summary>
/// Data record to hold data about Transitions that can be used to construct UI actions
/// </summary>
/// <param name="ActionType"></param>
/// <param name="Name"></param>
/// <param name="States"></param>
/// <param name="Policy"></param>
public record OwsWorkflowProcessData(Type ActionType, string Name, IEnumerable<StateCode> States, string? Policy = null);
