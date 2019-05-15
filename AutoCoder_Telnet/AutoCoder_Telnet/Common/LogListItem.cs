using AutoCoder.Core.Interfaces;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public class LogListItem : ILogItem
  {
    private Direction _Direction;
    public Direction Direction
    {
      get { return _Direction; }
      set
      {
        _Direction = value;
      }
    }

    public LogItemSpecial? LogSpecial
    { get; set; }

    public string Text
    { get; set; }

    public DateTime LogTime
    { get; set; }

    public string LogText
    { get { return this.ToString(); } }

    public byte[] Chunk
    { get; set; }

    private string _ChunkText;
    public string ChunkText
    {
      get { return _ChunkText; }
      set { _ChunkText = value; }
    }

    public bool IsImportant
    { get; set; }
    public string MergeName
    { get; set; }

    /// <summary>
    /// this item is start of a new group in the list of items. When listing the
    /// items should space a line or write a line of separator dashes.
    /// </summary>
    public bool NewGroup
    { get; set; }

    public LogListItem( LogListItem from = null)
    {
      if ( from != null)
      {
        this.Direction = from.Direction;
        this.LogSpecial = from.LogSpecial;
        this.LogTime = from.LogTime;
        this.Text = from.Text;
        this.Chunk = from.Chunk;
        this.ChunkText = from.ChunkText;
        this.IsImportant = from.IsImportant;
        this.MergeName = from.MergeName;
        this.NewGroup = from.NewGroup;
      }
    }
    public LogListItem(Direction Direction, string Text )
    {
      this.Direction = Direction;
      this.Text = Text;
      this.LogTime = DateTime.Now;
      this.IsImportant = false;
    }
    public LogListItem(Direction Direction, string Text, bool NewGroup)
      : this(Direction, Text)
    {
      this.NewGroup = NewGroup;
    }

    public LogListItem(Direction Direction, byte[] Chunk, string ChunkText )
    {
      this.Direction = Direction;
      this.Chunk = Chunk;
      this.ChunkText = ChunkText;
      this.LogTime = DateTime.Now;
    }

    public LogListItem(LogItemSpecial LogSpecial)
    {
      this.LogSpecial = LogSpecial;
      this.LogTime = DateTime.Now;
    }

    public ILogItem NewCopy()
    {
      var item = new LogListItem(this);
      return item;
    }

    public ILogItem NewItem( )
    {
      var item = new LogListItem( );
      return item;
    }

    public override string ToString()
    {
      if (this.LogSpecial != null)
        return this.LogSpecial.Value.ToString();
      else if (this.Chunk != null)
      {
        var sb = new StringBuilder();
        sb.Append(this.ChunkText);
        if (sb.Length > 0)
          sb.Append(" ");
        sb.Append(this.Chunk.ToHex(' '));
        return this.Direction.ToString() + " " + sb.ToString();
      }
      else
      {
        return this.Direction.ToString( ) + " " + this.Text;
      }
    }
  }
}
