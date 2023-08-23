/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.ComponentModel.DataAnnotations;

namespace Blazr.Flux.Web;

public record DboWorkflowLog
{
    [Key] public Guid Uid { get; init; }
    public Guid StateUid { get; init; }
    public int InitialState { get; init; }
    public int FinalState { get; init; }
    public string? Log { get; init; }
    public DateTimeOffset TimeStamp { get; init; }
    public Guid CreatorUid { get; init; }

    public static DboWorkflowLog ToDbo(WorkflowLog record)
        => new DboWorkflowLog()
        {
            Uid = record.WorkflowLogUid.Value,
            StateUid = record.StateUid.Value,
            InitialState = record.InitialState.Value,
            FinalState = record.FinalState.Value,
            Log = record.Log,
            TimeStamp = record.TimeStamp,
            CreatorUid = record.CreatorUid.Value,
        };

    public WorkflowLog FromDbo()
        => new WorkflowLog()
        {
            WorkflowLogUid = new(this.Uid),
            EntityState = new(StateCodes.Existing),
            StateUid = new(this.StateUid),
            InitialState = VisitorStateCodes.GetVisitorCode(this.InitialState),
            FinalState = VisitorStateCodes.GetVisitorCode(this.FinalState),
            Log = this.Log,
            TimeStamp = this.TimeStamp,
            CreatorUid = new(this.CreatorUid)
        };
}
