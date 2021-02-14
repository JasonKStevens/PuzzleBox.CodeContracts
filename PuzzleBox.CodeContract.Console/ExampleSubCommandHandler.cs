using System;
using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public class ExampleSubCommandHandler : CommandHandler<ExampleSubCommand, ExampleResult>
  {
    private readonly Contract<ExampleSubCommand, ExampleResult> _promise;

    protected override Contract<ExampleSubCommand, ExampleResult> DeclareContract()
    {
      var contract = new Contract<ExampleSubCommand, ExampleResult>()
        .Preconditions(AssertPreconditionsAsync)
        .Postonditions(AssertPostconditionsAsync)
        .Throws<ArgumentNullException>()
        .Behavior(ExecuteInner);

      return contract;
    }

    private Task<ExampleResult> ExecuteInner(ExampleSubCommand command)
    {
      Promise.Log(LogSeverity.Information, "Subcontract");
      return Task.FromResult(new ExampleResult());
    }

    private Task AssertPreconditionsAsync(ExampleSubCommand command)
    {
      return Task.CompletedTask;
    }

    private Task AssertPostconditionsAsync(ExampleResult result)
    {
      return Task.CompletedTask;
    }
  }
}
