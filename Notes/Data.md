## Domain Objects

All Entity objects implement the `IOwsState` interface:

```csharp
public interface IOwsState
{
    StateUid StateUid { get; }
`    StateCode StateCode { get; }
}
```

`StateUid` is a data object based on a `Guid`:

```csharp
public readonly record struct StateUid(Guid Value)
{
    public bool IsEmpty => this.Value == Guid.Empty;

    public static StateUid Empty => new StateUid(Guid.Empty);
}
```

`StateCode` is a data object based on an `int`.

```csharp
public readonly record struct StateCode(int Value, string Title);
```

In our example project `Visitor` is defined as :

```csharp
public readonly record struct VisitorUid(Guid Value);

public record Visitor : IOwsState, IOwsWorkflowState
{
    public VisitorUid VisitorUid { get; init; }
    public StateUid StateUid => new(VisitorUid.Value);
    public StateCode StateCode { get; init; }
    public string Name { get; init; } = "Not Set";
    public DateTimeOffset? OnSite { get; init; }
    public DateTimeOffset? OffSite { get; init; }
}
```
