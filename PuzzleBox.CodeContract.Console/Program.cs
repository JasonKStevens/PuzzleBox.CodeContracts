using System;
using System.Threading.Tasks;
using PuzzleBox.CodeContract.Console;

namespace PuzzleBox.CodeContract
{
  class Program
  {
    static async Task Main(string[] args)
    {
      // Arrange
      var subCommandHandler = new ExampleSubCommandHandler();
      var commandHandler = new MoneyTransferCommandHandler(subCommandHandler);
      var command = new TransferMoneyCommand
      {
        SourceAccount = "Alice",
        DestinationAccount = "Bob",
        Amount = 300
      };

      // A normal call
      var result1 = await commandHandler.ExecuteAsync(command);

      // Call to return a promise
      var promise = commandHandler.Execute(command);

      promise.LogStream
        .Subscribe(l => System.Console.WriteLine(l));

      var result2 = await promise.ResultTask;
    }
  }
}
