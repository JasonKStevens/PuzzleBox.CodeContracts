using System;
using System.Collections.Generic;

namespace PuzzleBox.CodeContract
{
  public class Contract<TCommand, TResult>
  {
    private readonly IDictionary<string, Func<TCommand, bool>> _assertPreconditions;
    private readonly IDictionary<string, Func<TResult, bool>> _assertPostconditions;
    private readonly ICollection<Type> _throws;

    public Contract()
    {
      _assertPreconditions = new Dictionary<string, Func<TCommand, bool>>();
      _assertPostconditions = new Dictionary<string, Func<TResult, bool>>();
      _throws = new List<Type>();
    }

    public Contract<TCommand, TResult> Requires(string assertion, Func<TCommand, bool> assertPreconditions)
    {
      _assertPreconditions.Add(assertion, assertPreconditions);
      return this;
    }

    public Contract<TCommand, TResult> Ensures(string assertion, Func<TResult, bool> assertPostconditions)
    {
      _assertPostconditions.Add(assertion, assertPostconditions);
      return this;
    }

    public Contract<TCommand, TResult> Throws<TException>(string when = null) // TODO
    {
      _throws.Add(typeof(TException));
      return this;
    }

    public Promise<TCommand, TResult> CreatePromise()
    {
      var promise = new Promise<TCommand, TResult>(
        _assertPreconditions,
        _assertPostconditions,
        _throws
      );
      return promise;
    }
  }
}
