using System;
using System.Runtime.Serialization;

namespace PuzzleBox.CodeContract.Exceptions
{
  [Serializable]
  internal class PreconditionFailedException : ContractBrokenException
  {
    private Exception ex;

    public PreconditionFailedException()
    {
    }

    public PreconditionFailedException(Exception ex)
    {
      this.ex = ex;
    }

    public PreconditionFailedException(string message) : base(message)
    {
    }

    public PreconditionFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected PreconditionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}