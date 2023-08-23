/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web;

public record DboVisitor 
{
    public Guid Uid { get; init; }
    public int StateCode { get; init; }
    public string? Name { get; init; }
    public DateTimeOffset? OnSite { get; init; }
    public DateTimeOffset? OffSite { get; init; }

    public static DboVisitor ToDbo(Visitor record)
        => new() {
            Uid = record.StateUid.Value,
            StateCode = record.EntityState.StateCode.Value,
            Name = record.Name,
            OnSite = record.OnSite,
            OffSite = record.OffSite,
        };

    public Visitor FromDbo()
        => new() {
            VisitorUid = new(this.Uid),
            EntityState = new(VisitorStateCodes.GetVisitorCode(this.StateCode)),
            Name = this.Name ?? "Not Set",
            OnSite = this.OnSite,
            OffSite = this.OffSite,
        };
}
