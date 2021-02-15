namespace PuzzleBox.CodeContract
{
  public class MoneyTransferredEvent
  {
    public TransferMoneyCommand Command { get; init; }
    public string SourceAccount { get; init; }
    public string DestinationAccount { get; init; }
    public decimal AmountTransferred { get; set; }
    public decimal OriginalSourceBalance { get; set; }
    public decimal SourceBalance { get; set; }
    public decimal OriginalDestinationBalance { get; set; }
    public decimal DestinationBalance { get; set; }
  }
}
