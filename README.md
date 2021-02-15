# PuzzleBox.CodeContracts

## Overview
This project is to test out an idea that combines Design by Contract with Promises.

## Why
I like the idea of being able to log the execution of a single server call and have detailed information of the call streamed back to the client.

The intention is that the logs relate to the contract and behaviour, but _not_ the implementation.  I imagine things like contract fulfilment, progress, decision points, and side effects.

It will be interesting to see if this helps improve code correctness and reduce the need for unit testing in some areas.

## Description
Each command handler defines its contract: preconditions, postconditions and exceptions thrown.  Invariants have been left for the moment because they assume the logic is OO and it may not be.

A promise is returned by the handler and allows the caller to receive updates on how the call is progressing, when it completes and when it errors.  Errors could be of type `BrokenContractException`.

Promises can be chained with sub-contracts as command handlers call command handlers.

```c#
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
;
```

```c#
// Call to return a promise
var promise = commandHandler.Execute(command);

promise.LogStream
  .Subscribe(l => Console.WriteLine(l));

result = await promise.ResultTask;
```
