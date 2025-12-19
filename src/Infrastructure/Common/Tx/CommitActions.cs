// Infrastructure/Common/Tx/CommitActions.cs
using Application.Common.Tx;

namespace Infrastructure.Common.Tx;

public sealed class CommitActions : ICommitActions
{
  private readonly List<Func<CancellationToken, Task>> _actions = new();
  public void Enqueue(Func<CancellationToken, Task> action) => _actions.Add(action);
  public IReadOnlyList<Func<CancellationToken, Task>> DequeueAll()
  {
    var copy = _actions.ToList();
    _actions.Clear();
    return copy;
  }
}
