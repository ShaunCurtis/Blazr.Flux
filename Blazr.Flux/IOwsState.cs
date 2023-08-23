/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux
{
    public interface IOwsState
    {
        EntityUid StateUid { get; }
        EntityState EntityState { get; }
    }
}
