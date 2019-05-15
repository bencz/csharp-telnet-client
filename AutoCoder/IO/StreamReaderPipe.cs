using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace AutoCoder.IO
{
  /// <summary>
  /// class that enables input from a StreamReader to be captured and stored by a
  /// background thread. The thread that actually wants to read from the stream
  /// can poll, peek or wait on a "data has arrived from the stream" event.
  /// </summary>
  public class StreamReaderPipe
  {

    Mutex QueuedOutputLock;
    AutoResetEvent QueuedOutputAvailable;

    public List<string> QueuedOutput
    {
      get;
      set;
    }

    private int OutputLocation
    { get; set; }

    public StreamReader OutputStream 
    { get ; set ; }

    public StreamReaderPipe(StreamReader OutputStream)
    {
      this.OutputStream = OutputStream;
      this.QueuedOutputLock = new Mutex();
      this.QueuedOutputAvailable = new AutoResetEvent(true);

      // list in which output that arrives from the output pipe will be queued.
      this.QueuedOutput = new List<string>();
      this.OutputLocation = 0;

      // start the thread which reads from the output stream and writes to the
      // output queue.
      ReadOutputThread();
    }

    /// <summary>
    /// method called to read the next line of text received by the background
    /// reader thread from the stream.
    /// </summary>
    /// <param name="Wait"></param>
    /// <returns></returns>
    public string GetLine(int? MillisecondsTimeout = null)
    {
      string gotLine = null ;

      bool gotLock = false;
      try
      {
        gotLock = this.QueuedOutputLock.WaitOne();
      }
      finally
      {
        if (gotLock == true)
        {
          if (MillisecondsTimeout == null)
          {
            this.QueuedOutputLock.ReleaseMutex();
          }
          else
          {
            WaitHandle.SignalAndWait(
              this.QueuedOutputLock, this.QueuedOutputAvailable, 
              MillisecondsTimeout.Value,
              false);
          }
        }
      }

      lock (OutputStream)
      {
        if (OutputLocation >= QueuedOutput.Count)
        {
          this.QueuedOutput.Clear();
          this.OutputLocation = 0;
          gotLine = null;
        }
        else
        {
          gotLine = this.QueuedOutput[this.OutputLocation];
          this.OutputLocation += 1;
        }
      }

      return gotLine;
    }

    void ReadOutputThread()
    {
      ThreadPool.QueueUserWorkItem(
        (o) =>
        {
          int bufferSize = 10240;
          char[] buffer = new char[bufferSize];
          while (true)
          {
            int count = this.OutputStream.Read(buffer, 0, bufferSize);
            if (count > 0)
            {
              string bufString = new string(buffer, 0, count);
              string[] lines = bufString.Split(
                new string[] { Environment.NewLine }, StringSplitOptions.None);

              foreach (var line in lines)
              {
                if (line != String.Empty)
                {
                  bool gotLock = false;
                  try
                  {
                    gotLock = this.QueuedOutputLock.WaitOne();
                    QueuedOutput.Add(line);
                    this.QueuedOutputAvailable.Set();
                  }
                  finally
                  {
                    if (gotLock == true)
                      this.QueuedOutputLock.ReleaseMutex();
                  }
                }
              }

            }
          }
        });
    }
  }
}
