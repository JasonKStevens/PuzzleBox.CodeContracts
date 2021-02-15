using PuzzleBox.CodeContract.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public class Promise<TCommand, TResult> : IPromise<TCommand, TResult>
  {
    private readonly IDictionary<string, Func<TCommand, bool>> _assertPreconditions;
    private readonly IDictionary<string, Func<TResult, bool>> _assertPostconditions;
    private readonly ICollection<Type> _throws;

    public Task<TResult> ResultTask { get; private set; }

    private readonly ReplaySubject<LogEvent> _logSubject;
    public IObservable<LogEvent> LogStream => _logSubject;

    public Promise(
      IDictionary<string, Func<TCommand, bool>> assertPreconditions,
      IDictionary<string, Func<TResult, bool>> assertPostconditions,
      ICollection<Type> throws
      )
    {
      _assertPreconditions = assertPreconditions;
      _assertPostconditions = assertPostconditions;
      _throws = throws;

      _logSubject = new ReplaySubject<LogEvent>();
      _throws = new List<Type>();
    }

    public void Execute(Func<TCommand, Task<TResult>> executeInner, TCommand command)
    {
      ResultTask = ExecuteTaskAsync(executeInner, command);
    }

    public void AddChildPromise(IPromise childPromise)
    {
      childPromise.LogStream
        .Subscribe(_logSubject);
    }

    private async Task<TResult> ExecuteTaskAsync(Func<TCommand, Task<TResult>> executeInner, TCommand command)
    {
      TResult result;

      try
      {
        AssertPreconditions(command);
        result = await ExecuteAsync(executeInner, command);
        AssertPostconditionsAsync(result);
      }
      catch (Exception ex)
      {
        LogException(ex);
        throw;
      }

      _logSubject.OnCompleted();
      return result;
    }

    private async Task<TResult> ExecuteAsync(Func<TCommand, Task<TResult>> executeInner, TCommand command)
    {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      TResult result;

      try
      {
        result = await executeInner(command);
      }
      catch (Exception ex)
      {
        var contractedToThrow = _throws.Contains(ex.GetType());
        var toThrow = contractedToThrow ? ex : new ContractBrokenException($"Not contracted to throw exception of type {ex.GetType().FullName}", ex);
        
        if (contractedToThrow)
          throw;
        throw toThrow;
      }

      stopwatch.Stop();
      Log(LogSeverity.Information, $"Task executed {stopwatch.ElapsedMilliseconds}ms");
      return result;
    }

    private void LogException(Exception ex)
    {
      Log(LogSeverity.Error, $"Execution failed: {ex.Message}");
      _logSubject.OnError(ex);
    }

    private void AssertPreconditions(TCommand command)
    {
      foreach (var precondition in _assertPreconditions)
      {
        var passed = precondition.Value(command);
        if (!passed)
          throw new PreconditionFailedException(precondition.Key);
      }

      Log(LogSeverity.Information, "Preconditions passed");
    }

    private void AssertPostconditionsAsync(TResult result)
    {
      foreach (var postcondition in _assertPostconditions)
      {
        var passed = postcondition.Value(result);
        if (!passed)
          throw new PostconditionFailedException(postcondition.Key);
      }

      Log(LogSeverity.Information, "Postconditions passed");
    }

    public void Log(LogSeverity severity, string message)
    {
      var logEvent = new LogEvent(severity, message);
      _logSubject.OnNext(logEvent);
    }
  }
}
