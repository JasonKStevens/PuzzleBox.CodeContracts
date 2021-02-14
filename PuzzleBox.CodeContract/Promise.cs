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
    private readonly Func<TCommand, Task> _assertPreconditions;
    private readonly Func<TResult, Task> _assertPostconditions;
    private readonly ICollection<Type> _throws;
    private readonly Func<TCommand, Task<TResult>> _executeTask;

    public Task<TResult> ResultTask { get; private set; }

    private readonly ReplaySubject<LogEvent> _logSubject;
    public IObservable<LogEvent> LogStream => _logSubject;

    public Promise(
      Func<TCommand, Task> assertPreconditions,
      Func<TResult, Task> assertPostconditions,
      ICollection<Type> throws,
      Func<TCommand, Task<TResult>> executeTask
      )
    {
      _assertPreconditions = assertPreconditions;
      _assertPostconditions = assertPostconditions;
      _throws = throws;
      _executeTask = executeTask;

      _logSubject = new ReplaySubject<LogEvent>();
      _throws = new List<Type>();
    }

    public void Execute(TCommand command)
    {
      ResultTask = ExecuteTaskAsync(command);
    }

    public void AddChildPromise(IPromise childPromise)
    {
      childPromise.LogStream
        .Subscribe(_logSubject);
    }

    private async Task<TResult> ExecuteTaskAsync(TCommand command)
    {
      await AssertPreconditions(command);
      var result = await ExecuteAsync(command);
      await AssertPostconditionsAsync(result);

      _logSubject.OnCompleted();
      return result;
    }

    private async Task<TResult> ExecuteAsync(TCommand command)
    {
      Log(LogSeverity.Debug, "Executing task");

      var stopwatch = new Stopwatch();
      stopwatch.Start();

      TResult result;

      try
      {
        result = await _executeTask(command);
      }
      catch (Exception ex)
      {
        var contractedToThrow = _throws.Contains(ex.GetType());
        var toThrow = contractedToThrow ? ex : new ContractBrokenException($"Not contracted to throw exception of type {ex.GetType().FullName}", ex);
        
        LogException(toThrow);

        if (contractedToThrow)
          throw;
        throw toThrow;
      }

      stopwatch.Stop();
      Log(LogSeverity.Debug, $"Task executed {stopwatch.ElapsedMilliseconds}ms");
      return result;
    }

    private void LogException(Exception ex)
    {
      Log(LogSeverity.Error, $"Execution failed: {ex.Message}");
      _logSubject.OnError(ex);
    }

    private async Task AssertPreconditions(TCommand command)
    {
      Log(LogSeverity.Debug, "Asserting preconditions");

      try
      {
        await _assertPreconditions(command);
      }
      catch (Exception ex)
      {
        Log(LogSeverity.Error, $"Precondition failed: {ex.Message}");
        _logSubject.OnError(ex);
        throw new PreconditionFailedException(ex);
      }

      Log(LogSeverity.Debug, "Preconditions passed");
    }

    private async Task AssertPostconditionsAsync(TResult result)
    {
      Log(LogSeverity.Debug, "Asserting postconditions");

      try
      {
        await _assertPostconditions(result);
      }
      catch (Exception ex)
      {
        Log(LogSeverity.Error, $"Postcondition failed: {ex.Message}");
        _logSubject.OnError(ex);
        throw new PostconditionFailedException(ex);
      }

      Log(LogSeverity.Debug, "Preconditions passed");
    }

    public void Log(LogSeverity severity, string message)
    {
      var logEvent = new LogEvent(severity, message);
      _logSubject.OnNext(logEvent);
    }
  }
}
