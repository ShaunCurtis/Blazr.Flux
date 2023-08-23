/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

/// <summary>
/// Value object for the User Uid
/// which is a Guid
/// </summary>
/// <param name="Value"></param>
public readonly record struct UserUid(Guid Value);
