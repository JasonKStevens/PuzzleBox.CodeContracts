using System;

namespace PuzzleBox.CodeContract
{
  public class TransferMoneyCommand : ICommand
  {
    public Guid CommandId { get; init; }
    public string SourceAccount { get; init; }
    public string DestinationAccount { get; init; }
    public decimal Amount { get; init; }
  }
}
