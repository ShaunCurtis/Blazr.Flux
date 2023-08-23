/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Flux;

/// <summary>
/// Internal object to hold an individual entity's State.
/// Provides the services to transition from the current to the new state
/// </summary>
/// <typeparam name="TState"></typeparam>
internal class OwsState<TState>
    where TState : class, IOwsState
{
    private Task _queueTask = Task.CompletedTask;
    private TaskCompletionSource<TState> _taskCompletionSource = new();
    private readonly Queue<OwsStateMutationDelegate<TState>> _mutationQueue = new();

    /// <summary>
    /// The StateUid (a Guid) for the State
    /// </summary>
    public EntityUid StateUid { get; private init; }

    /// <summary>
    /// The current state
    /// </summary>
    public TState State { get; private set; }

    /// <summary>
    /// The Task that represents current state mutation activity
    /// If it not completed then a mutatiuon is taking place
    /// </summary>
    public Task<TState> StateMutationTask => _taskCompletionSource.Task;

    /// <summary>
    /// The last access/mutation activity on the State
    /// Used by the mian state object to determine when to dispose the entity state
    /// </summary>
    internal DateTimeOffset LastActivity = DateTimeOffset.Now;

    /// <summary>
    /// Constructor
    /// Requires a state object to populate
    /// </summary>
    /// <param name="state"></param>
    public OwsState(TState state)
    {
        this.StateUid = state.StateUid;
        this.State = state;
        _taskCompletionSource.SetResult(state);
        this.LastActivity = DateTimeOffset.Now;
    }

    /// <summary>
    /// Primary method to apply a mutation to the state
    /// </summary>
    /// <param name="mutation">The OwsStateMutationDelegate to run to mutate the object</param>
    /// <returns></returns>
    public Task<TState> DispatchAsync(OwsStateMutationDelegate<TState> mutation)
    {
        _mutationQueue.Enqueue(mutation);
        if (_queueTask.IsCompleted)
            _queueTask = ServiceQueueAsync();

        return this.StateMutationTask;
    }

    // Called when an item is added to the mutation queue
    // but ServiceQueueAsync is not running
    // by testing the state of the task associated with _taskCompletionSource
    private async Task ServiceQueueAsync()
    {
        _taskCompletionSource = new();

        while (_mutationQueue.Count > 0)
        {
            var acton = _mutationQueue.Dequeue();
            var result = await acton.Invoke( new(State));
            if (result.Successful && result.State is not null)
                this.State = result.State;
        }

        _taskCompletionSource?.SetResult(State);
        LastActivity = DateTimeOffset.Now;
    }
}