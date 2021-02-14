using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public class Contract<TCommand, TResult>
  {
    private Func<TCommand, Task> _assertPreconditions;
    private Func<TResult, Task> _assertPostconditions;
    private ICollection<Type> _throws;
    private Func<TCommand, Task<TResult>> _executeTask;

    public Contract()
    {
      _throws = new List<Type>();
    }

    public Contract<TCommand, TResult> Preconditions(Func<TCommand, Task> assertPreconditions)
    {
      _assertPreconditions = assertPreconditions;
      return this;
    }

    public Contract<TCommand, TResult> Postonditions(Func<TResult, Task> assertPostconditions)
    {
      _assertPostconditions = assertPostconditions;
      return this;
    }

    public Contract<TCommand, TResult> Throws<TException>()
    {
      _throws.Add(typeof(TException));
      return this;
    }

    public Contract<TCommand, TResult> Behavior(Func<TCommand, Task<TResult>> executeTask)
    {
      _executeTask = executeTask;
      return this;
    }

    public Promise<TCommand, TResult> CreatePromise()
    {
      var promise = new Promise<TCommand, TResult>(
        _assertPreconditions,
        _assertPostconditions,
        _throws,
        _executeTask
      );
      return promise;
    }
  }
}
