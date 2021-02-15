# PuzzleBox.CodeContracts

## Overview
This is a WIP project to work through the idea of combining [Design by Contract](https://en.wikipedia.org/wiki/Design_by_contract) and [Promises](https://en.wikipedia.org/wiki/Futures_and_promises).  The contract defines the specification of the routine and the promise monitors its fulfillment, streaming events back to the client in real-time.

Inspired by Eiffel, the intention of contracts and promises is that they relate to the behaviour of their routine and _not_ its implementation.  This means that the contract is not only a formal specification of behaviour, it also defines what the unit tests should be.  Testing side-effects are the exception to this.

Promises can be chained to "subcontracts", so the one promise being returned captures all events from the contract and subcontracts.

## Description
Each command handler defines its contract: preconditions, postconditions and exceptions thrown.

```c#
var contract = new Contract<TransferMoneyCommand, MoneyTransferredResult>()
  // Preconditions
  .Requires(
    "Amount to transfer must be greater that zero",
    c => c.Amount > 0)
  .Requires(
    "Source account cannot be blank",
    c => !string.IsNullOrWhiteSpace(c.SourceAccount))
  .Requires(
    "Destination account cannot be blank",
    c => !string.IsNullOrWhiteSpace(c.DestinationAccount))

  // Postconditions
  .Ensures(
    "Amount transferred is the same requested amount.",
    e => e.Command.Amount == e.AmountTransferred)
  .Ensures(
    "Money deducted from source account matches requested amount.",
    e => e.SourceBalance == e.OriginalSourceBalance - e.Command.Amount)
  .Ensures(
    "Money added to destination account matches requested amount.",
    e => e.DestinationBalance == e.OriginalDestinationBalance + e.Command.Amount)

  // Exceptions thrown when ...
  .Throws<NotFoundException>("Source account not found")
  .Throws<NotFoundException>("Destination account not found")
;
```

A promise is returned by the handler and allows the caller to receive updates on how the call is progressing, when it completes and when it errors.  Errors could be of type `BrokenContractException`.

```c#
var promise = commandHandler.Execute(command);

promise.LogStream
  .Subscribe(
    l => System.Console.WriteLine(l),
    ex => System.Console.WriteLine($"Error: {ex.Message}"),
    () => System.Console.WriteLine("Complete")
  );

var result = await promise.ResultTask;
```

## Future
Explore distributed calls and contracts.  So when a command handler is responsible for a distributed transaction say, or calls out to other subcontracted routines that are running on other machines or processes.
