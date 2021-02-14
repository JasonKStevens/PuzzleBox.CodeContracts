using System;

namespace PuzzleBox.CodeContract
{
  class Program
  {
    static async System.Threading.Tasks.Task Main(string[] args)
    {
      // Arrange
      var subCommandHandler = new ExampleSubCommandHandler();
      var commandHandler = new ExampleCommandHandler(subCommandHandler);
      var command = new ExampleCommand{ CommandId = Guid.NewGuid() };

      // A normal call
      var result = await commandHandler.ExecuteAsync(command);

      // Call to return a promise
      var promise = commandHandler.Execute(command);

      promise.LogStream
        .Subscribe(l => Console.WriteLine(l));

      result = await promise.ResultTask;
    }
  }
}
