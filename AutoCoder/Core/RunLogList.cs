using AutoCoder.Core.Interfaces;
using AutoCoder.Ext.System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core
{
  public class RunLogList : List<RunLogMessage>, ILogList,  IEnumerable<RunLogMessage>
  {
    public string ListName
    { get; private set; }
    public string TextDesc
    { get; set; }
    private object LockFlag
    { get; set; }
    public RunLogList( string ListName = "" )
    {
      this.LockFlag = new object();
      this.ListName = ListName;
    }
    public void Add( string Text )
    {
      lock(this.LockFlag)
      {
        var msg = new RunLogMessage(Text);
        base.Add(msg);
      }
    }
    public void Add(string Text, object Tag)
    {
      lock (this.LockFlag)
      {
        var msg = new RunLogMessage(Text);
        msg.Tag = Tag;
        base.Add(msg);
      }
    }

    public IEnumerable<string> TimedMessageLines( )
    {
      foreach( var item in this)
      {
        var text = item.TimedMessageLine();
        yield return text;
      }
      yield break;
    }

    public void AddItem(ILogItem item)
    {
      Add(item as RunLogMessage);
    }

    public IEnumerable<ILogItem> LogItems( )
    {
      return this;
    }
    public ILogList NewList(string ListName = "")
    {
      return new RunLogList(ListName);
    }

    public ILogItem GetItem(int index)
    {
      return this[index];
    }

    IEnumerator<ILogItem> ILogList.GetEnumerator()
    {
      return base.GetEnumerator();
    }
  }
}
