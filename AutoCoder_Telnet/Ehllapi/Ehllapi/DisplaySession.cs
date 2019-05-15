using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoCoder.Ehllapi;
using Ehllapi;
using AutoCoder.Ehllapi.Settings;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi
{
  public class DisplaySession : IDisposable
  {
    string mActiveSessId = null;
    PresentationSpaceDim mDim = null;

    public DisplaySession()
    {
    }

    public DisplayLocation CursorLocation
    {
      get
      {
        ThrowSessionNotActive("Get CursorLocation failed.");
        Ehllapier.Wait(mActiveSessId);
        return Ehllapier.QueryCursorLocation( mActiveSessId );
      }
    }

    public PresentationSpaceDim Dim
    {
      get { return mDim; }
    }

    public string SessId
    {
      get { return mActiveSessId; }
    }

    public bool SystemAvailable
    {
      get
      {
        OperatorInfoArea oia = GetOperatorInfoArea();
        return oia.SystemAvailable;
      }
    }

    void IDisposable.Dispose()
    {
      if (mActiveSessId != null)
      {
        try
        {
          Disconnect();
        }
        catch
        {
          mActiveSessId = null;
        }
      }
    }

    public void Connect(string SessId)
    {
      Ehllapier.Connect(SessId);
      mActiveSessId = SessId;
      mDim = new PresentationSpaceDim(24, 80);
    }

    public string Copy_OIA()
    {
      string psData = Ehllapier.Copy_OIA( mActiveSessId ) ;
      return psData;
    }

    public string CopyPresentationSpaceToString()
    {
      string psData = Ehllapier.CopyPresentationSpaceToString(mActiveSessId);
      return psData;
    }

    public void Disconnect()
    {
      if (mActiveSessId == null)
        throw new EhllapiExcp("display session is not active");

      Ehllapier.Disconnect(mActiveSessId);
      mActiveSessId = null;
    }

    public OperatorInfoArea GetOperatorInfoArea()
    {
      ThrowSessionNotActive("GetShiftState failed.");
      byte[] oiaBytes = Ehllapier.GetOiaBytes(mActiveSessId);
      return new OperatorInfoArea(oiaBytes);
    }

    public PresentationSpace GetPresentationSpace( )
    {
      Ehllapier.Wait(mActiveSessId);
      var ps = Ehllapier.GetPresentationSpace(this);
      return ps;
    }

    public void SendKeys(string InKeys)
    {
      Ehllapier.Wait(mActiveSessId);
      Ehllapier.SendKeys(mActiveSessId, InKeys);
    }

    public void SendKeys(KeyboardKey Key)
    {
      Ehllapier.Wait(mActiveSessId);
      Ehllapier.SendKeys(mActiveSessId, Key.KeyText);
    }

    public static DisplaySession StartSession(string SessId)
    {
      var settings = EhllapiSettings.RecallSettings();
      
      var sess = new DisplaySession()
      {
        mActiveSessId = SessId
      };



      return sess;
    }

    public void TabRightToColumn( int InColNx )
    {
      bool hasTabbedToTop = false;
      DisplayLocation startLoc = null;
      while (true)
      {
        DisplayLocation loc = CursorLocation;
        if (loc.Column == InColNx)
          break;

        // have tabbed to the top of the screen.
        if ((hasTabbedToTop == false) &&
          (startLoc != null) &&
          (loc <= startLoc))
          hasTabbedToTop = true;

        // have tabbed full circle back to start loc.
        if ((startLoc != null) &&
          (hasTabbedToTop == true) &&
          (loc >= startLoc))
        {
          throw new EhllapiExcp("Tab to display location not found");
        }

        if (startLoc == null)
          startLoc = loc;
        SendKeys(Ehllapier.Key.TabRight);
      }
    }

    public void TabRightToDisplayLocation(DisplayLocation InLoc)
    {
      bool hasTabbedToTop = false;
      DisplayLocation startLoc = null;
      while (true)
      {
        DisplayLocation loc = CursorLocation;
        if (loc == InLoc)
          break;

        // have tabbed to the top of the screen.
        if ((hasTabbedToTop == false) &&
          (startLoc != null) &&
          (loc <= startLoc))
          hasTabbedToTop = true;

        // have tabbed full circle back to start loc.
        if ((startLoc != null) &&
          (hasTabbedToTop == true) &&
          (loc >= startLoc))
        {
          throw new EhllapiExcp("Tab to display location not found");
        }

        if (startLoc == null)
          startLoc = loc;
        SendKeys(Ehllapier.Key.TabRight);
      }
    }

    public void ThrowSessionNotActive( string InText )
    {
      if ( mActiveSessId == null )
        throw new EhllapiExcp( "Display session not active. " + InText ) ;
    }

    public void Wait()
    {
      Ehllapier.Wait(mActiveSessId);
      
      // ehllapi wait for host is sometimes not good enough. the new screen
      // from the host may not have arrived yet at the display. Use a dummy
      // read of the presentation space to make sure there is not 
      // "wait for host response" going on. ( see CopyPresentationSpaceToString
      // in the Ehllapier class. )
      GetPresentationSpace();
    }

  }
}
