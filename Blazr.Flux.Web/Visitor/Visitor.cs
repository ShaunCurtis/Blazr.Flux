/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web;

public readonly record struct VisitorUid(Guid Value);

public record Visitor : IOwsState
{
    public VisitorUid VisitorUid { get; init; }
    public EntityState EntityState { get; init; }
    public string Name { get; init; } = "Not Set";
    public DateTimeOffset? OnSite { get; init; }
    public DateTimeOffset? OffSite { get; init; }

    public EntityUid StateUid => new(VisitorUid.Value);
}
