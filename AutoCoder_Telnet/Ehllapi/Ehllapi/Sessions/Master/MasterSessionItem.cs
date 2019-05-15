using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Api.Structs;
using AutoCoder.Ehllapi.Api;
using Ehllapi.Messages;
using Ehllapi.Api.Host;

namespace AutoCoder.Ehllapi.Sessions.Master
{
  /// <summary>
  /// core info about the client access session.
  /// </summary>
  public class MasterSessionItem
  {

    string _LongName;
    public string LongName
    {
      get { return _LongName; }
      set
      {
        // BgnTemp
        if (value == null)
        {
          char bb = 'a';
        }
        // EndTemp
        _LongName = value;
      }
    }

    public string SessId
    { get; set; }

    /// <summary>
    /// unique id of this session. Not supplied by client access, assigned and used
    /// internally. 
    /// </summary>
    public Guid ItemId
    { get; private set; }

    /// <summary>
    /// the client access session window/process has ended. Functions which return a
    /// list of sessions no longer show the sessid.
    /// </summary>
    public bool IsEnded
    { get; set; }

    /// <summary>
    /// the hWnd of the client access session window.
    /// </summary>
    public IntPtr hSessWnd
    { get; set; }

    /// <summary>
    /// The GetWindowText of the client access session. Retrieved using a call to the 
    /// host access class library.
    /// </summary>
    public string WindowText
    { get; set; }

    public string ErrorText
    { get; set; }

    public MasterSessionItem()
    {
      this.SessId = null;
      this.ErrorText = null;
      this.hSessWnd = IntPtr.Zero;
      this.WindowText = null;
      this.ItemId = Guid.NewGuid();
    }

    public MasterSessionItem(SessInfo Info)
      : this(Info.SessId) 
    {
    }

    public MasterSessionItem(string SessId)
      : this( ) 
    {
      this.SessId = SessId;

      this.WindowText = HostAccess.GetSessionTitle(SessId);
      this.hSessWnd = HostAccess.GetSessionHwnd(SessId);

      var sa = Ehllapier.QuerySessionStatus(this.SessId);
      this.LongName = sa.LongName;
    }

    public MasterSessionItem(MasterSessionItem Item)
      :this( )
    {
      this.ItemId = Guid.NewGuid();
      this.SessId = Item.SessId;
      this.hSessWnd = Item.hSessWnd;
      this.WindowText = Item.WindowText;
      this.LongName = Item.LongName;
    }
  }
}
