using System;
using System.Runtime.Serialization;

namespace PuzzleBox.CodeContract.Exceptions
{
  [Serializable]
  internal class ContractBrokenException : Exception
  {
    public ContractBrokenException()
    {
    }

    public ContractBrokenException(string message) : base(message)
    {
    }

    public ContractBrokenException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ContractBrokenException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}