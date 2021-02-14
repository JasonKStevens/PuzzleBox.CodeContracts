using System;
using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public class ExampleCommandHandler : CommandHandler<ExampleCommand, ExampleResult>
  {
    private readonly ICommandHandler<ExampleSubCommand, ExampleResult> _subCommandHandler;

    public ExampleCommandHandler(
      ICommandHandler<ExampleSubCommand, ExampleResult> subCommandHandler
    ) : base()
    {
      _subCommandHandler = subCommandHandler;
    }

    protected override Contract<ExampleCommand, ExampleResult> DeclareContract()
    {
      var contract = new Contract<ExampleCommand, ExampleResult>()
        .Preconditions(AssertPreconditionsAsync)
        .Postonditions(AssertPostconditionsAsync)
        .Throws<ArgumentException>()
        .Throws<ArgumentNullException>()
        .Behavior(ExecuteInner);

      return contract;
    }

    private async Task<ExampleResult> ExecuteInner(ExampleCommand command)
    {
      Promise.Log(LogSeverity.Information, "Long operation 1");
      await Task.Delay(500);

      Promise.Log(LogSeverity.Information, "Long operation 2");
      await Task.Delay(500);
      
      var result = await _subCommandHandler.ExecuteAsync(new ExampleSubCommand(), Promise);
      return result;
    }

    private Task AssertPreconditionsAsync(ExampleCommand command)
    {
      if (command.CommandId == Guid.Empty)
        throw new ArgumentException("CommandId cannot be empty");

      return Task.CompletedTask;
    }

    private Task AssertPostconditionsAsync(ExampleResult result)
    {
      return Task.CompletedTask;
    }
  }
}
