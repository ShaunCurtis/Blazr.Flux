# Blazr.Flux

Blazr.Flux is an experimental basic workflow framework for DotNetCore.  The demonstration project is a Blazor Server application.

Blazr.Flux is based on the *Redux* and *Flux* patterns and various implementations such as Fluxor.

Some of the principles:

1. Data objects are immutable, defined as `record` with `{set; init;}` properties.
2. The store contains the only version of the truth.
3. You get a reference to the truth when you get the data object from the store.  That only represents the truth at the time you get it.
4. You hook up to change events to get a new reference to the latest version of the truth when the object mutates.
5. Mutations are `async`.
6. Stores control entities by `EntityUid`, which is based on a `Guid`.  The `Visitor` store may contain serveral `Visitor` instances, but each will be an individual visitor with a unique `EntityUid`. 
7. A `Process` is dispatched to the Store with an `EntityUid`.  The store instance runs the `OwsStateMutationDelegate` it has been passed.  Any mutations are notified through the store's `StateHasChanged` event.

The basic sequence is:

1. Logic within the application creates an `IOwsProcessRequest` instance and applies instance information to the process.
 
2. It dispatches the process to the DI registered `IProcessRequestBroker`.

3. The broker resolves the correct `IProcessHandler` for the process, and dispatches the process instance to the handler.

4. The handler calls `Dispatch` on the store.  It passes the dispatcher the `EntityUid` (from the `IOwsProcessRequest` instance) and a reference to it's `ExecuteAsync` as a `OwsStateMutationDelegate` delagate.
 
5. The Store finds the `EntityState` and dispatches the delegate to the `OwsState` instance.

6. `OwsState` queues the delegate and awaits the completion of the queue cycle. 


## Example Implementation of One Way Street

First the Visitor record we want to apply the workflow to.

```csharp

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
```

And a log record to log activity.

```csharp

public readonly record struct WorkflowLogUid(Guid Value);

public record WorkflowLog
{
    public WorkflowLogUid WorkflowLogUid { get; init; }
    public StateCode StateCode { get; init; } = StateCodes.New;
    public EntityUid StateUid { get; init; }
    public StateCode InitialState { get; init; }
    public StateCode FinalState { get; init; }
    public string? Log { get; init; }
    public DateTimeOffset TimeStamp { get; init; }
    // Not implemented here
    public UserUid CreatorUid { get; init; }
    public string? Creator { get; init; }
}
```

### States

Each state is defined by a `StateCode` which is a simple `struct`:

```
public readonly record struct StateCode(int Value, string Title );
```

Define `VisitorStateCodes` to hold our `StateCode` objects.  `Draft` is set to the same code as the base class's `Existing`.

```csharp
public class VisitorStateCodes : StateCodes
{
    public static StateCode Draft = new(StateCodes.Existing.Value, "Draft");
    public static StateCode Submitted = new(20, "Submitted");
    public static StateCode Approved = new(30, "Approved");
    public static StateCode OnSite = new(40, "On Site");
    public static StateCode OffSite = new(50, "Off Site");
    public static StateCode Completed = new(1001, "Completed");
    public static StateCode Closed = new(1002, "Closed");


    public static List<StateCode> VisitorStateCodeList = new()
    {
        New, Draft, Submitted, Approved, OnSite, OffSite, Completed, Closed, Null
    };

    public static StateCode GetVisitorCode(int code)
        => VisitorStateCodeList.FirstOrDefault(item => item.Value == code);
}
```

### Processes

Processes define activities we apply to our workflow object.  In general this is transitioning state, but it may also be other activities such as updating information. 

We define an empty `IVisitorWorkflowProcess` interface to make it easy to find all the process classes specific to our workflow.

```csharp
public interface IVisitorWorkflowProcess {}
```

Define each process as a class.  Here's the `SaveToDraftProcess`.  Note it inherits from `OwsWorkflowProcess` and implements the `IOwsWorkflowProcess` and `IVisitorWorkflowProcess` interfaces.

It defines which starting states can apply the process and the state that the process will be mutated to.

```csharp
public class SaveToDraftProcess : OwsWorkflowProcess, IOwsWorkflowProcess, IVisitorWorkflowProcess
{
    public SaveToDraftProcess() {
        this.DisplayName = "Save As Draft";
        this.Description = "Save the Visit as a draft for later submission.";
        this.StartingStates = new() { VisitorStateCodes.New, VisitorStateCodes.Draft };
        this.FinalState = VisitorStateCodes.Draft;
    }
}
```

Here's the process for booking the visitor on site.

```csharp
public class OnSiteProcess : OwsWorkflowProcess, IOwsWorkflowProcess, IVisitorWorkflowProcess
{
    public OnSiteProcess()
    {
        this.DisplayName = "On Site";
        this.Description = "Book the visitor on site.";
        this.StartingStates = new() { VisitorStateCodes.Approved, VisitorStateCodes.OffSite };
        this.FinalState = VisitorStateCodes.OnSite;
    }
}
```

And the process to close a visit because the visitor isn't coming.

```csharp
public class ClosedProcess : OwsWorkflowProcess, IOwsWorkflowProcess, IVisitorWorkflowProcess
{
    public ClosedProcess()
    {
        this.DisplayName = "Close";
        this.Description = "The visit should be closed.  The visitor isn't coming.";
        this.StartingStates = new() { VisitorStateCodes.Draft, VisitorStateCodes.Submitted, VisitorStateCodes.Approved };
        this.FinalState = VisitorStateCodes.Closed;
    }
}
```

## Workflow Handlers

These define the nitty gritty.  What happens whwn you click a button to progress a workflow.

This is the base handler for the Visitor.  There's two parts:

1. Do the state mutation.
2. Save the Visitor and Create and save a WorkflowLog.

```csharp
public class VisitorWorkflowHandler : IProcessHandler<Visitor>
{
    private readonly OwsEntityStore<Visitor> _store;
    private readonly ICommandHandler<Visitor> _visitorCommandHandler;
    private readonly ICommandHandler<WorkflowLog> _workflowLogCommandHandler;

    protected IOwsWorkflowProcess? Process;

    public VisitorWorkflowHandler(OwsEntityStore<Visitor> store, ICommandHandler<Visitor> visitorCommandHandler, ICommandHandler<WorkflowLog> workflowCommandHandler)
    {
        _store = store;
        _visitorCommandHandler = visitorCommandHandler;
        _workflowLogCommandHandler = workflowCommandHandler;
    }

    public async ValueTask<Visitor> DispatchAsync<TProcess>(TProcess process) where TProcess : class, IOwsWorkflowProcess
    {
        this.Process = process;
        return await _store.DispatchAsync(process.StateUid, this.ExecuteAsync);
    }

    public async ValueTask<Visitor> DispatchAsync(IOwsWorkflowProcess process)
    {
        this.Process = process;
        return await _store.DispatchAsync(process.StateUid, this.ExecuteAsync);
    }

    protected virtual Task<Visitor> ExecuteMutationAsync(Visitor state)
    {
        // do the mutation
    }

    protected async virtual Task<bool> ExecutePersistAsync(Visitor previousState, Visitor newState)
    {
        // save the data
    }

    private async Task<OwsMutationResult<Visitor>> ExecuteAsync(OwsMutationRequest<Visitor> request)
    {
        if (this.Process is null)
            return OwsMutationResult<Visitor>.Failure("No Process defined", request.State with { });

        // TODO - build in error code if things go wrong
        var newState = await this.ExecuteMutationAsync(request.State);

        await this.ExecutePersistAsync(request.State, newState);
 
        return OwsMutationResult<Visitor>.Success(newState);
    }
}
```

The mutation simply builds a new record with the state changed to the process object's `FinalState`.  It's `virtual` so we can override it where we need to.

```csharp
    protected virtual Task<Visitor> ExecuteMutationAsync(Visitor state)
    {
        if (this.Process is null)
            return Task.FromResult(state);

        var newState = state with { EntityState = state.EntityState.Mutate(this.Process!.FinalState) };

        return Task.FromResult(newState);
    }
```

The persistence uses the data pipeline handlers to save the visitor, create a workflow log and save it. 

```csharp
    protected async virtual Task<bool> ExecutePersistAsync(Visitor previousState, Visitor newState)
    {
        if (this.Process is null)
            return false;

        // Create the log
        var log = new WorkflowLog
        {
            WorkflowLogUid = new(Guid.NewGuid()),
            StateUid = previousState.StateUid,
            CreatorUid = new(Guid.Empty),
            InitialState = previousState.EntityState.StateCode,
            FinalState = this.Process.FinalState,
            StateCode = StateCodes.New,
            Log = $"Changed State from {previousState.EntityState.StateCode.Title} to {this.Process.FinalState.Title}",
            TimeStamp = DateTime.UtcNow
        };

        //Normally a DB Transaction
        {
            await _visitorCommandHandler.ExecuteAsync(newState);

            await _workflowLogCommandHandler.ExecuteAsync(log);
        }

        return true;
    }
```

Several of the processes have some extra activity in the mutation process.

`OnSiteWorkflowHandler` needs to set the time/date when the visitor is booked on site.  It overrides `ExecuteMutationAsync`, reads the date provided in the process instance and sets `OnSite` in `Visitor`.

```csharp
public class OnSiteWorkflowHandler : VisitorWorkflowHandler, IProcessHandler<Visitor, OnSiteProcess>
{
    public OnSiteWorkflowHandler(OwsEntityStore<Visitor> store, ICommandHandler<Visitor> visitorCommandHandler, ICommandHandler<WorkflowLog> workflowCommandHandler) 
        : base(store, visitorCommandHandler, workflowCommandHandler) { }

    public async ValueTask<Visitor> DispatchAsync(OnSiteProcess process)
        => await base.DispatchAsync(process);

    protected override Task<Visitor> ExecuteMutationAsync(Visitor state)
    {
        var newEntityState = state.EntityState with { StateCode = this.Process?.FinalState ?? state.EntityState.StateCode };

        var newState = state with
        {
            EntityState = newEntityState,
            OnSite = this.Process?.TimeDate
        };

        return Task.FromResult(newState);
    }
}
```

`OffSiteWorkflowHandler` and `CompletedWorkflowHandler` are similar.

## Process Handler Resolver

The Process Handler Resolver resolves which process handler to use for a process.

The pattern is:

```
```csharp
public class XXXXProcessHandlerResolver : IProcessHandlerResolver<XXXX>
{
    private readonly IServiceProvider _serviceProvider;
    private IProcessHandler<Visitor> _processHandler;

    public VisitorProcessHandlerResolver(IServiceProvider serviceProvider, IProcessHandler<Visitor> processHandler)
    { 
        _serviceProvider = serviceProvider;
        _processHandler = processHandler;
    }

    public IProcessHandler<Visitor> GetHandler<TProcess>(TProcess process) where TProcess : class, IOwsWorkflowProcess
    {
        IProcessHandler<XXXX>? handler = null;

        // assign custom handler 

        return handler ?? _processHandler;
    }
}
```
```

Our resolver looks like this:

```csharp
public class VisitorProcessHandlerResolver : IProcessHandlerResolver<Visitor>
{
    private readonly IServiceProvider _serviceProvider;
    private IProcessHandler<Visitor> _processHandler;

    public VisitorProcessHandlerResolver(IServiceProvider serviceProvider, IProcessHandler<Visitor> processHandler)
    { 
        _serviceProvider = serviceProvider;
        _processHandler = processHandler;
    }

    public IProcessHandler<Visitor> GetHandler<TProcess>(TProcess process) where TProcess : class, IOwsWorkflowProcess
    {
        IProcessHandler<Visitor>? handler = null;

        if (process is OnSiteProcess)
            handler = _serviceProvider.GetService<IProcessHandler<Visitor, OnSiteProcess>>() as IProcessHandler<Visitor>;

        if (process is OffSiteProcess)
            handler = _serviceProvider.GetService<IProcessHandler<Visitor, OffSiteProcess>>() as IProcessHandler<Visitor>;

        if (process is CompletedProcess)
            handler = _serviceProvider.GetService<IProcessHandler<Visitor, CompletedProcess>>() as IProcessHandler<Visitor>;

        return handler ?? _processHandler;
    }
}
```

## Presentation

We can now define our Presenter for the UI component. It:

1. Gets the `Visitor` from the database and loads it into the Entity Store.
2. Gets the Workflow logs.
3. Gets the Processes that are applicable to the current `Visitor` State
4. Provides a method to run a process.

```csharp
public class VisitorWorkflowManager
{
    private OwsEntityStore<Visitor> _store;
    private IItemHandler<Visitor> _visitorItemHandler;
    private IListHandler<WorkflowLog> _workflowLogListHandler;
    private WorkflowProcessBroker<Visitor> _processBroker;
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

    public VisitorWorkflowManager(OwsEntityStore<Visitor> store, IItemHandler<Visitor> itemHandler, WorkflowProcessBroker<Visitor> workflowProcessBroker, IListHandler<WorkflowLog> listHandler)
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

    public async Task RunTransition(Type processType)
    {
        var process = Activator.CreateInstance(processType) as IOwsWorkflowProcess;

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

        var actions = this.GetAllTypesThatImplementInterface<IVisitorWorkflowProcess>();

        foreach (var action in actions)
        {
            var instance = Activator.CreateInstance(action) as IOwsWorkflowProcess;
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
```

## Index

And fianlly a demo page, which looks deceptively simple.

```csharp
@page "/"
@inject VisitorWorkflowManager WorkflowManager
<PageTitle>Index</PageTitle>

<h1>Visitor Workflow Demo</h1>

<div class="mb-3 text-end">
    @foreach (var action in WorkflowManager.GetActionsForState() )
    {
        <button class="btn btn-primary ms-1" @onclick="() => RunTransition(action.ActionType)" >@action.Name</button>
    }
</div>
<div class="bg-dark text-white m-2 p-1">
    <pre class="ps-2">Name : @WorkflowManager.Context.Name</pre>
    <pre class="ps-2">State : @WorkflowManager.Context.EntityState.StateCode.Title</pre>
    <pre class="ps-2">On Site : @WorkflowManager.Context.OnSite</pre>
    <pre class="ps-2">Off Site : @WorkflowManager.Context.OffSite</pre>
</div>

<div class="bg-dark text-white m-2 p-1">
    @foreach(var log in WorkflowManager.Logs)
    {
        <pre class="ps-2">@GetLogText(log)</pre>
    }
</div>

@code {
    protected async override Task OnInitializedAsync()
        => await this.WorkflowManager.LoadAsync(Guid.Empty);

    private async Task RunTransition(Type transition)
        => await this.WorkflowManager.RunTransition(transition);   

    private string GetLogText(WorkflowLog log)
        => $"{log.TimeStamp.DateTime.ToLongTimeString()} - {log.InitialState.Title} -> {log.FinalState.Title} - {log.Log}";
}
```
