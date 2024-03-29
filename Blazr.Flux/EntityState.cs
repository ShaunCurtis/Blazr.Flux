﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Flux;

public readonly record struct EntityState
{
    public StateCode StateCode { get; init; }
    public bool IsMutated { get; init; }
    public bool MarkedForDeletion { get; init; }

    public bool IsNew => StateCode.Value == 0;

    public EntityState(StateCode stateCode)
    {
        StateCode = stateCode;
    }

    public EntityState Mutate()
        => this with { IsMutated = true };

    public EntityState Mutate(StateCode stateCode)
        => this with { StateCode = stateCode, IsMutated = true };

    public EntityState MarkForDeletion()
        => this with { MarkedForDeletion = true };
}
