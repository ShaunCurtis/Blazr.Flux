/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web;

public readonly record struct WorkflowLogUid(Guid Value);

public record WorkflowLog : IOwsState
{
    public WorkflowLogUid WorkflowLogUid { get; init; }
    public EntityState EntityState { get; init; }
    public StateCode StateCode { get; init; } = StateCodes.New;
    public EntityUid StateUid { get; init; }
    public StateCode InitialState { get; init; }
    public StateCode FinalState { get; init; }
    public string? Log { get; init; }
    public DateTimeOffset TimeStamp { get; init; }
    public UserUid CreatorUid { get; init; }
    public string? Creator { get; init; }
}
