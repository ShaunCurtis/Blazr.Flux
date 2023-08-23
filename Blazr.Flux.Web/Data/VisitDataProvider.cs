/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web.Data
{
    public class VisitDataProvider
    {
        private DboVisitor _visitor;

        private List<DboWorkflowLog> _log;

        public VisitDataProvider()
        {
            _visitor = new()
            {
                Name = "Shaun Curtis",
                StateCode = 1,
                Uid = Guid.NewGuid()
            };

            _log = new List<DboWorkflowLog>() {
                new() {
                    Uid = Guid.NewGuid(),
                    InitialState = 0,
                    FinalState = 1,
                    CreatorUid = Guid.Empty,
                    StateUid = _visitor.Uid,
                    Log = "Created and Saved",
                    TimeStamp = DateTime.Now,
                }
            };
        }

        public async ValueTask<Visitor> GetVisitorAsync(Guid uid)
        {
            await Task.Yield();
            return _visitor.FromDbo();
        }

        public async ValueTask VisitorCommandAsync(Visitor item)
        {
            await Task.Yield();
            _visitor = DboVisitor.ToDbo(item);
        }

        public async ValueTask<IEnumerable<WorkflowLog>> GetLogsAsync(EntityUid entityUid)
        {
            await Task.Yield();
            var list =  _log.Where(item => item.StateUid == entityUid.Value).Select(item => item.FromDbo()).ToList();
            return list.AsEnumerable();
        }

        public async ValueTask WorkflowLogCommand(WorkflowLog item)
        {
            await Task.Yield();
            _log.Add(DboWorkflowLog.ToDbo(item));
        }
    }
}
