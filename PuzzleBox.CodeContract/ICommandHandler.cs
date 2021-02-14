using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
  {
    IPromise<TCommand, TResult> Execute(TCommand command, IPromise parentPromise = null);
    Task<TResult> ExecuteAsync(TCommand command, IPromise parentPromise = null);
  }
}