using System;

namespace PuzzleBox.CodeContract
{
  public interface ICommand
  {
    Guid CommandId { get; }
  }
}