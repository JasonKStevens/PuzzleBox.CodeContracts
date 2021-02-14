using System;

namespace PuzzleBox.CodeContract
{
  public class ExampleSubCommand : ICommand
  {
    public Guid CommandId { get; init; }
  }
}
