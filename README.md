# PuzzleBox.CodeContracts

The purpose of this project is to work through the idea of combining [Design by Contract](https://en.wikipedia.org/wiki/Design_by_contract) (DbC) and [Promises](https://en.wikipedia.org/wiki/Futures_and_promises).  The contract defines the specification of the routine and the promise monitors its fulfillment, streaming events back to the client.

## Overview
C# doesn't have native support for DbC, like Eiffel or Scala for example, but there are some initiatves for it with [Code Contracts](https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/code-contracts) and [Spec#](https://en.wikipedia.org/wiki/Spec_Sharp).

The intention of a contract is that it relates to the routine's behaviour and not its implementation.  Since this is also what unit tests are supposed to test, it follows that unit tests should test the contract.  Testing side-effects are the exception to this - maybe they're something to add to the contract or separate from the routine under contract.

Streaming events back to the client of the routine can be handy sometimes.  It's also nice not to have a library tied to a particular logging technology and instead leave it up to the client to do the logging from the events that are streamed back.  This streaming may also open up calls to a more conversational style API.

Promises can be chained to "subcontracts", so the one promise being returned captures all events from the contract and subcontracts.

## Description
Each command handler defines its contract: preconditions, postconditions and exceptions thrown. The terminology is borrowed from Eiffel.

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

  // Exceptions thrown
  .Asserts<NotFoundException>("Source account not found")
  .Asserts<NotFoundException>("Destination account not found")
;
```

A promise is returned by the handler and allows the caller to receive updates on how the call is progressing, when it completes and when it errors.  Errors could be of type `BrokenContractException`.

```c#
var promise = commandHandler.Execute(command);

promise.LogStream
  .Subscribe(
    l => Console.WriteLine(l),
    ex => Console.WriteLine($"Error: {ex.Message}"),
    () => Console.WriteLine("Complete")
  );

var result = await promise.ResultTask;
```

## Future
Explore how contracts handle distributed calls.
See where this fits in with BDD.
