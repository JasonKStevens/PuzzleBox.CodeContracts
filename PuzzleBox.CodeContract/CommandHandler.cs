using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
  {
    protected Contract<TCommand, TResult> Contract { get; private set; }
    protected Promise<TCommand, TResult> Promise { get; private set; }

    protected CommandHandler()
    {
      Contract = DeclareContract();
    }

    protected abstract Contract<TCommand, TResult> DeclareContract();
    protected abstract Task<TResult> ExecuteInner(TCommand command);

    public IPromise<TCommand, TResult> Execute(TCommand command, IPromise parentPromise = null)
    {
      Promise = Contract.CreatePromise();
      parentPromise?.AddChildPromise(Promise);

      Promise.Execute(ExecuteInner, command);
      return Promise;
    }

    public Task<TResult> ExecuteAsync(TCommand command, IPromise parentPromise = null)
    {
      return Execute(command, parentPromise).ResultTask;
    }
  }
}
