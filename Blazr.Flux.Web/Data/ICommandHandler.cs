/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web.Data;

public interface ICommandHandler<TRecord>
     where TRecord : class, IOwsState
{
    public ValueTask<CommandResult> ExecuteAsync(TRecord record);
}
