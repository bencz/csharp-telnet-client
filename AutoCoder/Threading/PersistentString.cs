using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Threading;
using System.IO;
using AutoCoder.Text;
using System.Threading;
using AutoCoder.Core;
using AutoCoder.Ext;
using AutoCoder.Ext.System;

namespace AutoCoder.Threading
{
  /// <summary>
  /// A string stored in a persistent backing file.
  /// The name of the persistent string is used as the name of backing file.
  /// By default, the backing file is stored in the Path.GetTempPath directory.
  /// The value of the persistent string persists only as long as the backing
  /// file exists.
  /// </summary>
  public class PersistentString : IWriteStatusMessage 
  {
    string _BackingDir = null ;

    /// <summary>
    /// System wide name of the string.
    /// </summary>
    public string Name { get; set; }

    public PersistentString(string Name)
    {
      this.Name = Name;
    }

    public string BackingDir
    {
      get
      {
        if ( _BackingDir == null )
          _BackingDir = Path.GetTempPath( ) ;
        return _BackingDir ;
      }
      set { _BackingDir = value ; }
    }

    public string BackingFilePath
    {
      get
      {
        if (Name.IsNullOrEmpty())
          throw new ApplicationException("PersistentString name is blank");
        string fileName = this.Name + ".txt";
        string backPath = Path.Combine(this.BackingDir, fileName);
        return backPath;
      }
    }

    /// <summary>
    /// read and return the string stored in the backing file. 
    /// </summary>
    /// <returns></returns>
    public string Recall()
    {
      string text = null;
      using (SystemWideMutex locker = new SystemWideMutex(Name + "_mutex"))
      {
        locker.WaitOne();
        if (System.IO.File.Exists(this.BackingFilePath) == false)
          text = "";
        else
        {
          text = System.IO.File.ReadAllText(this.BackingFilePath);
        }
      }
      return text;
    }

    private SystemWideAutoResetEvent _StringStoredEvent;
    public SystemWideAutoResetEvent StringStoredEvent
    {
      get
      {
        if (_StringStoredEvent == null)
        {
          string eventName = this.Name + "_StringStoredEvent";
          _StringStoredEvent = new SystemWideAutoResetEvent(false, eventName);
        }
        return _StringStoredEvent;
      }
    }

    public static string Recall(string Name)
    {
      PersistentString ps = new PersistentString(Name);
      string s1 = ps.Recall();
      return s1;
    }

    /// <summary>
    /// Store the string to the named, persistent backing file.
    /// </summary>
    /// <param name="Text"></param>
    public void Store(string Text)
    {
      using (SystemWideMutex locker = new SystemWideMutex(Name + "_mutex"))
      {
        locker.WaitOne();
        System.IO.File.WriteAllText(this.BackingFilePath, Text);

        // signal an event indicating the string has been stored.
        StringStoredEvent.WaitHandle.Set();
      }
    }

    public static void Store(string Name, string Text)
    {
      PersistentString ps = new PersistentString(Name);
      ps.Store(Text);
    }

    public string Value
    {
      get { return Recall(); }
      set { Store(value); }
    }

    public string WaitStringStored(int WaitTime)
    {
      bool isSet = this.StringStoredEvent.WaitOne(WaitTime);
      if (isSet == false)
        return null;
      else
        return this.Recall();
    }

    /// <summary>
    /// IWriteStatusMessage is used as the interface for writing status messages
    /// to a runlog like object.
    /// </summary>
    /// <param name="Text"></param>
    #region IWriteStatusMessage Members

    public void WriteMessage(string Text)
    {
      this.Store(Text);
    }

    #endregion
  }
}
