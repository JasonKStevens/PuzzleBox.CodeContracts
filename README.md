# PuzzleBox.CodeContracts

## Overview
This project is to work through an idea that combines Design by Contract with Promises.  The contract defines the specification of the routine and the promise monitors its fulfillment.

Inspired by Eiffel, the intention of a contract (and the events emitted by the promise) is that it relates to the behaviour of the routine but _not_ its implementation.  This means that it's the contract that is all and only what needs unit testing.  The exception to this is when side-effects come into play where mocks are needed to validate them.

Promises can be chained to "subcontracts", so the one promise being returned captures all events from the contract and subcontracts.

## Why
Being able to log the execution of a single server call is useful, especially when the log is streamed back to the client in real-time before the routine has completed.

Logs are sometimes used as a poor replacement for debugging, so part of this project is to put development on rails to have the developer think more about the behaviour rather than the implementation.  The idea is to improve code correctness by making the specification formal and, in doing so, specify what the unit tests should be.

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
