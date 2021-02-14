# PuzzleBox.CodeContracts

## Overview
This project is to test out an idea that combines Design by Contract with Promises.

## Why
I like the idea of being able to log the single execution of a server call and have detailed information of the call returned to the client, for example a web page.  Essentially a "please explain" directive.

The intention is that the logging returned to the client pertains to the contract and behaviour, and not the implementation.  I imagine things like progress, key conditionals, side effects and contract fulfilment.

Combining a promise like this the declaration of a code contracts seems natural, or at least extends the meaning of "promise" to include the intent of fulfilling the contract.

It will be interesting to see if this helps improve code correctness and reduce the need for unit testing in some areas.

## Description
Each command handler defines its contract: preconditions, postconditions and exceptions thrown.  Invariants have been left for the moment because they would assume the logic is OO and it may not be.

A promise is returned by the handler and allows the caller to receive updates on how the call is progressing, when it completes and when it errors.  Errors could be of type `BrokenContractException`.

Promises can be chained with sub-contracts as command handlers call command handlers.

```
  var contract = new Contract<ExampleSubCommand, ExampleResult>()
    .Preconditions(AssertPreconditionsAsync)
    .Postonditions(AssertPostconditionsAsync)
    .Throws<ArgumentNullException>()
    .Behavior(ExecuteInner);
```

```
  // Call to return a promise
  var promise = commandHandler.Execute(command);

  promise.LogStream
    .Subscribe(l => Console.WriteLine(l));

  result = await promise.ResultTask;
```
