using PuzzleBox.CodeContract.Console.Exceptions;
using System.Threading.Tasks;

namespace PuzzleBox.CodeContract.Console
{
  public class MoneyTransferCommandHandler : CommandHandler<TransferMoneyCommand, MoneyTransferredEvent>
  {
    private readonly ICommandHandler<ExampleSubCommand, MoneyTransferredEvent> _subCommandHandler;

    public MoneyTransferCommandHandler(
      ICommandHandler<ExampleSubCommand, MoneyTransferredEvent> subCommandHandler
    ) : base()
    {
      _subCommandHandler = subCommandHandler;
    }

    protected override Contract<TransferMoneyCommand, MoneyTransferredEvent> DeclareContract()
    {
      var contract = new Contract<TransferMoneyCommand, MoneyTransferredEvent>()
        .Requires(
          "Amount to transfer must be greater that zero",
          c => c.Amount > 0)
        
        .Requires(
          "Source account cannot be blank",
          c => !string.IsNullOrWhiteSpace(c.SourceAccount))
        
        .Requires(
          "Destination account cannot be blank",
          c => !string.IsNullOrWhiteSpace(c.DestinationAccount))
        
        .Ensures(
          "Amount transferred is the same requested amount.",
          e => e.Command.Amount == e.AmountTransferred)
        
        .Ensures(
          "Money deducted from source account matches requested amount.",
          e => e.SourceBalance == e.OriginalSourceBalance - e.Command.Amount)

        .Ensures(
          "Money added to destination account matches requested amount.",
          e => e.DestinationBalance == e.OriginalDestinationBalance + e.Command.Amount)

        .Throws<NotFoundException>("Source account not found")
        .Throws<NotFoundException>("Destination account not found")

        //.Separate<IPaymentServices>()  // Remote services with no contract
        //.Separate<IRepository>()
      ;

      return contract;
    }

    protected override async Task<MoneyTransferredEvent> ExecuteInner(TransferMoneyCommand command)
    {
      Promise.Log(LogSeverity.Information, "Long operation 1");
      await Task.Delay(500);

      Promise.Log(LogSeverity.Information, "Long operation 2");
      await Task.Delay(500);
      
      // var result = await _subCommandHandler.ExecuteAsync(new ExampleSubCommand(), Promise);
      //var isSourceAccountFound = false;
      //Promise.Throw<NotFoundException>(isSourceAccountFound, "Source account not found");

      return new MoneyTransferredEvent
      {
        Command = command,
        OriginalSourceBalance = 1000,
        OriginalDestinationBalance = -50,
        SourceBalance = 1000 - command.Amount,
        DestinationBalance = -50 + command.Amount,
        AmountTransferred = command.Amount
      };
    }
  }
}
