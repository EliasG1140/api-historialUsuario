// Application/Common/Tx/ICommitActions.cs
namespace Application.Common.Tx;

public interface ICommitActions
{
  void Enqueue(Func<CancellationToken, Task> action);
  IReadOnlyList<Func<CancellationToken, Task>> DequeueAll();
}