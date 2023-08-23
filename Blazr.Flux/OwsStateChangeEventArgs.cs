/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public class OwsStateChangeEventArgs : EventArgs
{
    public object State { get; init; }
    public EntityUid StateUid { get; init; }

    public OwsStateChangeEventArgs(EntityUid stateUid, object state)
    {
        State = state;
        StateUid = stateUid;
    }
}
