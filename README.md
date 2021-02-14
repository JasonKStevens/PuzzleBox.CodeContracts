# PuzzleBox.CodeContracts

This project is to test out an idea that combines Design by Contract with Promises.

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
