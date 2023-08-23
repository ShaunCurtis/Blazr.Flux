/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux.Web;

public class VisitorWorkflowManager
{
    private OwsEntityStore<Visitor> _store;
    private IItemHandler<Visitor> _visitorItemHandler;
    private IListHandler<WorkflowLog> _workflowLogListHandler;
    private IProcessRequestBroker<Visitor> _processBroker;
    private List<OwsWorkflowProcessData> _allActions = new List<OwsWorkflowProcessData>();
    private EntityUid _stateUid = EntityUid.Empty;
    
    public IEnumerable<WorkflowLog> Logs { get; private set; } = Enumerable.Empty<WorkflowLog>();

    public Visitor Context { get; private set; } = new();
    
    public IEnumerable<OwsWorkflowProcessData> CurrentStateActions
    {
        get
        {
            if (_stateUid.IsEmpty)
                return Enumerable.Empty<OwsWorkflowProcessData>();

            return GetActionsForState();
        }
    }

    public VisitorWorkflowManager(OwsEntityStore<Visitor> store, IItemHandler<Visitor> itemHandler, IProcessRequestBroker<Visitor> workflowProcessBroker, IListHandler<WorkflowLog> listHandler)
    {
        _store = store;
        _processBroker = workflowProcessBroker;
        _visitorItemHandler = itemHandler;
        _workflowLogListHandler = listHandler;

        this.LoadWorkflowActions();
    }

    public async ValueTask LoadAsync(Guid uid)
    {
        var visitor = await _visitorItemHandler.GetItemAsync(new());

        this.Logs = await _workflowLogListHandler.GetItemsAsync(visitor.StateUid);

        if (visitor is not null)
        {
            _store.AddAState(visitor);
            this.Context = visitor;
            _stateUid = visitor.StateUid;
        }
    }

    public async Task DispatchProcess(Type processType)
    {
        var process = Activator.CreateInstance(processType) as IOwsProcessRequest;

        ArgumentNullException.ThrowIfNull(process);

        process.TimeDate = DateTimeOffset.Now;
        process.Data = null;
        process.StateUid = this.Context.StateUid;

        this.Context = await _processBroker.DispatchAsync(process);

        this.Logs = await _workflowLogListHandler.GetItemsAsync(this.Context.StateUid);
    }

    private void LoadWorkflowActions()
    {
        _allActions.Clear();

        var actions = this.GetAllTypesThatImplementInterface<IVisitorProcessRequest>();

        foreach (var action in actions)
        {
            var instance = Activator.CreateInstance(action) as IOwsProcessRequest;
            if (instance != null)
                _allActions.Add(new(action, instance.DisplayName, instance.StartingStates, instance.Policy));
        }
    }

    public IEnumerable<OwsWorkflowProcessData> GetActionsForState()
    {
        var state = _stateUid.IsEmpty 
            ? StateCodes.Null 
            : this.Context.EntityState.StateCode;

        var stateactions = _allActions.Where(item => item.States.Contains(state));
        
        return stateactions;
    }

    private IEnumerable<Type> GetAllTypesThatImplementInterface<T>()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(Visitor));
        if (assembly is not null)
            return assembly
                .GetTypes()
                .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);

        return Enumerable.Empty<Type>();
    }
}
