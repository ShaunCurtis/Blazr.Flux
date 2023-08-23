/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Flux;

public class OwsEntityStore<TRecord>
    where TRecord : class, IOwsState
{
    // Contains all the current state instancies
    private readonly List<OwsState<TRecord>> _States = new();
    private int _entityTimeout = 30;

    public event EventHandler<OwsStateChangeEventArgs>? StateHasChanged;

    /// <summary>
    /// Adds the initial state instance to the store
    /// and returns a handle to the instance
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public void AddAState(TRecord record)
    {
        this.ThrowIfExists(record);

        _States.Add(new OwsState<TRecord>(record));
    }

    /// <summary>
    /// Removes an instance from the Store
    /// </summary>
    /// <param name="uid"></param>
    public void RemoveAState(EntityUid uid)
    {
        var record = _States.FirstOrDefault(item => item.StateUid == uid);

        if (record != null)
            _States.Remove(record);
    }

    /// <summary>
    /// Gets a reference to the current state
    /// Note that this is only valid at the time of the request.
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public TRecord? GetState(EntityUid uid)
        => _States.FirstOrDefault(item => item.StateUid == uid)?.State ?? null;

    /// <summary>
    /// The dispatcher for the entity
    /// </summary>
    /// <param name="uid">The Uid of the entity</param>
    /// <param name="mutation">The mutation delegate to apply to the entity</param>
    /// <returns>The mutated State</returns>
    /// <exception cref="StateDoesNotExistsException">Raised if the entity does not exist in the store</exception>
    public async Task<TRecord> DispatchAsync(EntityUid uid, OwsStateMutationDelegate<TRecord> mutation)
    {
        var state = _States.FirstOrDefault(item => item.StateUid == uid);
        if (state is null)
            throw new StateDoesNotExistsException($"A state object does not exist for identity {uid}");

        var task = state.DispatchAsync(mutation);

        this.DoHousekeeping();

        var newState = await task;

        this.StateHasChanged?.Invoke(this, new OwsStateChangeEventArgs(uid, newState));

        return newState;
    }

    /// <summary>
    /// Methods to set the timeout period in minutes for an entity state
    /// Each transaction on an entity sets it's timestamp
    /// If an entity timeout expires the entitt object and all it's resources 
    /// will be deallocated for the GC to remove 
    /// </summary>
    /// <param name="minutes"></param>
    public void SetEntityTimeOut(int minutes)
        => _entityTimeout = minutes;

    private void DoHousekeeping()
    {
        // Get any states that haven't been accessed in the timneour period and remove them
        var deletes = _States.Where(item => item.LastActivity > DateTime.Now.AddMinutes(_entityTimeout));
        foreach (var item in deletes)
            _States.Remove(item);
    }

    private void ThrowIfExists(TRecord record)
    {
        if (_States.Any(item => item.StateUid == record.StateUid))
            throw new StateAlreadyExistsException($"A state object already exists for identity {record.StateUid}");
    }

    private void ThrowIfDoesNotExist(TRecord record)
    {
        if (!_States.Any(item => item.StateUid == record.StateUid))
            throw new StateDoesNotExistsException($"A state object does not exist for identity {record.StateUid}");
    }
}
