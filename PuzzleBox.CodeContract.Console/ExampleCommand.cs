using System;

namespace PuzzleBox.CodeContract
{
  public class ExampleCommand : ICommand
  {
    public Guid CommandId { get; init; }
  }
}
