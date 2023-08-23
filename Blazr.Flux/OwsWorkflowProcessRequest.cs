/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

/// <summary>
/// Abstract class providing the boilerplate code for an IOwsWorkflowTransition
/// </summary>
public abstract class OwsWorkflowProcessRequest 
{
    public EntityUid StateUid { get; set; }
    public StateCode FinalState { get; init; }
    public List<StateCode> StartingStates { get; init; } = new();
    public string DisplayName { get; init; } = "Not Set";
    public string Description { get; init; } = "Transition to next State";
    public string? Policy { get; init; }
    public string? Log { get; set; }
    public DateTimeOffset TimeDate { get; set; } = DateTimeOffset.Now;
    public object? Data { get; set; }
}
