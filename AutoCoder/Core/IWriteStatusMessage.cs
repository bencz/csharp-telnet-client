using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core
{
  /// <summary>
  /// IWriteStatusMessage is used as the interface for writing status messages
  /// to a runlog like object.
  /// </summary>
  public interface IWriteStatusMessage
  {
    void WriteMessage(string Text);
  }

  public interface IMessageWriter
  {
    void WriteTextMessage(string Text);
  }

  // extension methods of the IMessageWriter and IWriteStatusMessage interfaces.
  // Enables the method to test if the interface is null before calling the 
  // method on it. 
  public static class WriteStatusMessageExt
  {
    public static void Write( this IWriteStatusMessage Writer, string Text )
    {
      if ( Writer != null )
      {
        Writer.WriteMessage( Text ) ;
      }
    }

    public static void Write(this IMessageWriter Writer, string Text)
    {
      if (Writer != null)
      {
        Writer.WriteTextMessage(Text);
      }
    }

    public static void WriteTextMessage(this IMessageWriter Writer, string Text)
    {
      if (Writer != null)
      {
        Writer.WriteTextMessage(Text);
      }
    }
  }
}
