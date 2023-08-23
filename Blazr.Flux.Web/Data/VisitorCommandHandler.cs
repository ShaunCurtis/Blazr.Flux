/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web.Data;

public class VisitorCommandHandler : ICommandHandler<Visitor>
{
    private readonly VisitDataProvider _visitDataProvider;

    public VisitorCommandHandler(VisitDataProvider visitDataProvider)
    {
        _visitDataProvider = visitDataProvider;
    }

    public async ValueTask<CommandResult> ExecuteAsync(Visitor visitor)
    {
        // simplistic - normally need to implement create and update (and maybe delete) 
        // into the data store
        await _visitDataProvider.VisitorCommandAsync(visitor);
        return new CommandResult(Successful: true);
    }
}
