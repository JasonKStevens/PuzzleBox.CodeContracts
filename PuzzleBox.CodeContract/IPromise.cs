using System;
using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public interface IPromise<TCommand, TResult> : IPromise
  {
    Task<TResult> ResultTask { get; }
  }
  public interface IPromise
  {
    IObservable<LogEvent> LogStream { get; }
    void AddChildPromise(IPromise childPromise);
  }
}