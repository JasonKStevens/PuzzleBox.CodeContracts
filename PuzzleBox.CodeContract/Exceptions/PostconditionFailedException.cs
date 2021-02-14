using System;
using System.Runtime.Serialization;

namespace PuzzleBox.CodeContract.Exceptions
{
  [Serializable]
  internal class PostconditionFailedException : ContractBrokenException
  {
    private Exception ex;

    public PostconditionFailedException()
    {
    }

    public PostconditionFailedException(Exception ex)
    {
      this.ex = ex;
    }

    public PostconditionFailedException(string message) : base(message)
    {
    }

    public PostconditionFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected PostconditionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
