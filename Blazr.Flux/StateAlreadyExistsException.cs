/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

/// <summary>
/// Exception raised when the state already exists in the store
/// </summary>
public class StateAlreadyExistsException : Exception
{
    public StateAlreadyExistsException(string message) : base(message)
    { }
}
