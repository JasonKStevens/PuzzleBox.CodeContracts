using System;
using System.Threading.Tasks;

namespace PuzzleBox.CodeContract
{
  public class ExampleSubCommandHandler : CommandHandler<ExampleSubCommand, MoneyTransferredEvent>
  {
    protected override Contract<ExampleSubCommand, MoneyTransferredEvent> DeclareContract()
    {
      var contract = new Contract<ExampleSubCommand, MoneyTransferredEvent>()
        .Throws<ArgumentNullException>()
      ;

      return contract;
    }

    protected override Task<MoneyTransferredEvent> ExecuteInner(ExampleSubCommand command)
    {
      Promise.Log(LogSeverity.Information, "Subcontract");
      return Task.FromResult(new MoneyTransferredEvent());
    }
  }
}
