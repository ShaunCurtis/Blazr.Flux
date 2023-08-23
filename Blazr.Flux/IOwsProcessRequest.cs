/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

/// <summary>
/// Interface defining a Workflow Transition
/// </summary>
public interface IOwsProcessRequest
{
    EntityUid StateUid { get; set; }
    StateCode FinalState { get; init; }
    List<StateCode> StartingStates { get; init; }
    string DisplayName { get; init; }
    string Description  { get; init; }
    string? Policy { get; init; }
    string? Log { get; set; }
    DateTimeOffset TimeDate { get; set; }
    object? Data { get; set; }
}
