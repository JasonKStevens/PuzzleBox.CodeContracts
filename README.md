# PuzzleBox.CodeContracts

## Overview
This project is to work through an idea that combines [Design by Contract](https://en.wikipedia.org/wiki/Design_by_contract) with [Promises](https://en.wikipedia.org/wiki/Futures_and_promises).  The contract defines the specification of the routine and the promise monitors its fulfillment, streaming events back to the client in real-time.

Inspired by Eiffel, the intention of contracts and promises is that they relate to the behaviour of their routine and _not_ its implementation.  This means that the contract is not only a formal specification of behaviour, it also defines what the unit tests should be.  Testing side-effects are the exception to this.

Promises can be chained to "subcontracts", so the one promise being returned captures all events from the contract and subcontracts.

The main goals are:

* Provide real-time insights to the caller.
* Reduce overall development time by improving code correctness and reliability from the outset.
* Facilitate development rather than get in the way.

## Description
Each command handler defines its contract: preconditions, postconditions and exceptions thrown.

A promise is returned by the handler and allows the caller to receive updates on how the call is progressing, when it completes and when it errors.  Errors could be of type `BrokenContractException`.

Promises can be chained with subcontracts as command handlers call child command handlers.

```c#
var contract = new Contract<TransferMoneyCommand, MoneyTransferredEvent>()
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

```c#
// Calling the handler returns a promise
var promise = commandHandler.Execute(command);

promise.LogStream
  .Subscribe(l => Console.WriteLine(l));

result = await promise.ResultTask;
```

## Future
Explore distributed calls and contracts.  So when a command handler is responsible for a distributed transaction say, or calls out to other subcontracted routines that are running on other machines or processes.
