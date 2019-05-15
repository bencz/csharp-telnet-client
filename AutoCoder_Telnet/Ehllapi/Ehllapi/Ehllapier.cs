using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using AutoCoder;
using AutoCoder.Ehllapi.Presentation;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi
{

  public enum DisplayIntensity{ Normal, High, None } ;
  public enum FormatCode { PC3270, PC400, Vt, None } ;
  public enum BufferCharRepresentation { SingleByte, DoubleByte, None } ;

  public partial class Ehllapier
  {
    // the actual hllapi function that is exported by the pcshll32 dll.
    public class PcsDll
    {
      [DllImport("PCSHLL32.dll")]
      public static extern UInt32 hllapi(
        out UInt32 FuncNx, StringBuilder Data,
        out UInt32 DataLx, out UInt32 RetC);

      [DllImport("PCSHLL32.dll", EntryPoint = "hllapi")]
      public static extern UInt32 hllapi_Buffer(
        out UInt32 FuncNx, byte[] Buffer,
        out UInt32 BufferLx, out UInt32 RetC);
    }

    public class Key
    {
      public static string Enter
      {
        get { return "@E"; }
      }

      public static string FieldExit
      {
        get { return "@A@E"; }
      }

      public static string NewLine
      {
        get { return "@N"; }
      }

      public static string F3
      {
        get { return "@3"; }
      }

      public static string F4
      {
        get { return "@4"; }
      }

      public static string F6
      {
        get { return "@6"; }
      }

      public static string Shift
      {
        get { return "@S"; }
      }

      public static string TabLeft
      {
        get { return "@B"; }
      }

      public static string TabRight
      {
        get { return "@T"; }
      }

      public static string SystemRequest
      {
        get { return "@A@H"; }
      }
    }

    // ------------------------------- Connect -----------------------------
    public static void Connect(string SessId)
    {
      UInt32 rc;
      var sbData = new StringBuilder(4);
      sbData.Append(SessId);
      UInt32 retc = 0;
      UInt32 f = InputCodes.CONNECT_PS;
      UInt32 l = 4;
      rc = PcsDll.hllapi(out f, sbData, out l, out retc);
      if ((rc != 0) && (rc != 4) && (rc != 5))
        throw new EhllapiExcp("connect to session " + SessId + " failed");
    }

    public static string CopyFieldToString(
      string InSessId, DisplayLocation InLoc, int InFieldLx)
    {
      UInt32 psPos = DisplayLocationToPS(InSessId, InLoc);
      string fldText = CopyFieldToString(psPos, InFieldLx);
      return fldText;
    }

    public static string CopyFieldToString(UInt32 InPS_Pos, int InFieldLx)
    {
      byte[] buffer = null;

      buffer = new byte[InFieldLx];
      UInt32 rc = InPS_Pos;
      UInt32 funcNx = Ehllapier.InputCodes.CopyFieldToString;
      UInt32 bufLx = (uint)InFieldLx;
      UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, buffer, out bufLx, out rc);

      if (rv == 2)
        throw new EhllapiExcp("CopyFieldToString failed. Parameter error.");
      else if (rv == 7)
        throw new EhllapiExcp(
          "CopyFieldToString failed. Presentation space position not valid.");
      else if ((rv == 0) || (rv == 6))
      { }
      else
        throw new EhllapiExcp("CopyFieldToString failed.");

      string fldText = Encoding.ASCII.GetString(buffer);
      return fldText;
    }

    public static string Copy_OIA(string InSessId)
    {
      byte[] buffer = null;

      while (true)
      {
        buffer = new byte[104];
        UInt32 rc = 0;
        UInt32 funcNx = Ehllapier.InputCodes.Copy_OIA;
        UInt32 bufLx = (uint)buffer.Length;
        UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, buffer, out bufLx, out rc);

        // waiting for host response. likely, there is a new screen coming from
        // the host. wait until it arrives.
        if (rc == 4)
        {
          Thread.Sleep(1);
          continue;
        }
        else if ((rv != 0) || (rc != 0))
        {
          throw new EhllapiExcp(
            "Copy_OIA failed. Session " + InSessId +
            "  rv:" + rv.ToString() +
            "  rc:" + rc.ToString());
        }
        else
          break;
      }

      // return buffer bytes as unicode characters.
      System.Text.Encoding enc = System.Text.Encoding.ASCII;
      string uniBuf = enc.GetString(buffer);
      return uniBuf;
    }

    public static byte[] CopyPresentationSpace( int InSpaceSx )
    {
      byte[] buf = null;
      UInt32 rc = 0;

      while (true)
      {
        UInt32 funcNx = Ehllapier.InputCodes.CopyPresentationSpace;
        buf = new byte[InSpaceSx];
        UInt32 bufLx = (uint)buf.Length;
        UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, buf, out bufLx, out rc);

        // waiting for host response. likely, there is a new screen coming from
        // the host. wait until it arrives.
        if (rc == 4)
        {
          Thread.Sleep(1);
          continue;
        }
        else if ((rc == 0) || (rc == 5))
          break;
        else
        {
          throw new EhllapiExcp(
            "CopyPresentationSpace failed. " +
            "  rv:" + rv.ToString() +
            "  rc:" + rc.ToString());
        }
      }

      return buf;
    }

    public static string CopyPresentationSpaceToString(string InSessId)
    {
      UInt32 pos = 1;
      UInt32 len = 80 * 24;
      StringBuilder sbData = null;

      while (true)
      {
        sbData = new StringBuilder(3000);
        UInt32 rc = pos;
        UInt32 f = Ehllapier.InputCodes.HA_COPY_PS_TO_STR;
        UInt32 l = len;
        UInt32 rv = PcsDll.hllapi(out f, sbData, out l, out rc);

        // waiting for host response. likely, there is a new screen coming from
        // the host. wait until it arrives.
        if (rc == 4)
        {
          Thread.Sleep(1);
          continue;
        }
        else if ((rv != 0) || (rc != 0))
        {
          throw new EhllapiExcp(
            "copy_ps_to_str failed. Session " + InSessId +
            "  rv:" + rv.ToString() +
            "  rc:" + rc.ToString() +
            "  ps:" + sbData.ToString());
        }
        else
          break;
      }

      return sbData.ToString();
    }

    public static void Disconnect(string InSessId)
    {
      UInt32 rc;

      StringBuilder sbData = new StringBuilder(4);
      sbData.Append(InSessId);
      UInt32 retc = 0;
      UInt32 f = Ehllapier.InputCodes.DISCONNECT_PS;
      UInt32 l = 4;
      rc = PcsDll.hllapi(out f, sbData, out l, out retc);
      if (rc != 0)
        throw new EhllapiExcp("Disconnect from session " + InSessId + " failed");
    }

    /// <summary>
    /// convert DisplayLocation to presentation space position.
    /// </summary>
    /// <param name="InSessId"></param>
    /// <param name="InRow"></param>
    /// <param name="InCol"></param>
    /// <returns></returns>
    public static UInt32 DisplayLocationToPS(string InSessId, DisplayLocation InLoc)
    {
      string sp3 = "   "; // 3 spaces
      string fac2 = InSessId + sp3 + 'R' + sp3;
      Byte[] fac2Bytes = Encoding.ASCII.GetBytes(fac2);
      UInt32 funcNx = Ehllapier.InputCodes.ConvertPosition;
      UInt32 lgth = (UInt32)InLoc.Row;
      UInt32 rc = (UInt32)InLoc.Column;
      UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, fac2Bytes, out lgth, out rc);
      if (rv == 0)
        throw new EhllapiExcp("RowToPS failed. Incorrect row or column.");
      else if (rv == 9998)
        throw new EhllapiExcp("RowToPS failed. Incorrect presentation space ID.");
      else if (rv == 9999)
        throw new EhllapiExcp("RowToPS failed. Character 2 is not P or R.");

      return rv;
    }

    public static byte[] GetOiaBytes(string InSessId)
    {
      byte[] buffer = null;

      while (true)
      {
        buffer = new byte[104];
        UInt32 rc = 0;
        UInt32 funcNx = Ehllapier.InputCodes.Copy_OIA;
        UInt32 bufLx = (uint)buffer.Length;
        UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, buffer, out bufLx, out rc);

        // waiting for host response. likely, there is a new screen coming from
        // the host. wait until it arrives.
        if (rc == 4)
        {
          Thread.Sleep(1);
          continue;
        }
        else if ((rv != 0) || (rc != 0))
        {
          throw new EhllapiExcp(
            "Copy_OIA failed. Session " + InSessId +
            "  rv:" + rv.ToString() +
            "  rc:" + rc.ToString());
        }
        else
          break;
      }

      return buffer;
    }

    public static PresentationSpace GetPresentationSpace(DisplaySession Sess)
    {
      PresentationSpace ps = null;
      byte[] charBuf = null;
      int bufSx = 0;

      // get display characters and char attributes. 
      while (true)
      {
        Ehllapier.SetSessionParameters("ATTRB,EAB,NOXLATE");
        bufSx = (Sess.Dim.Width * Sess.Dim.Height) * 2;
        charBuf = CopyPresentationSpace(bufSx);

        // presentation space contains nulls.
        if ((charBuf[0] == 0x20) && (charBuf[1] == 0x00))
        {
          Thread.Sleep(1);
          continue;
        }

        // got some valid data
        break;
      }

      // get the color attributes of presentation space characters.
      Ehllapier.SetSessionParameters("ATTRB,EAB,XLATE");
      byte[] colorBuf = CopyPresentationSpace(bufSx);

      var lowPs = new LowPresentationSpace(Sess, charBuf, colorBuf, 2);
      ps = new PresentationSpace(lowPs);

      return ps;
    }

    /// <summary>
    /// call ehllapi api to convert presentation space position to DisplayLocation.
    /// </summary>
    /// <param name="InSessId"></param>
    /// <param name="InPsPos"></param>
    /// <returns></returns>
    public static DisplayLocation PS_ToDisplayLocation(
      string InSessId, UInt32 InPsPos)
    {
      string sp3 = "   "; // 3 spaces
      string fac2 = InSessId + sp3 + 'P' + sp3;
      Byte[] fac2Bytes = Encoding.ASCII.GetBytes(fac2);
      UInt32 funcNx = Ehllapier.InputCodes.ConvertPosition;
      UInt32 lgth = 0;
      UInt32 rc = (UInt32)InPsPos;
      UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, fac2Bytes, out lgth, out rc);
      if (rv == 0)
        throw new EhllapiExcp("RowToPS failed. Incorrect row or column.");
      else if (rv == 9998)
        throw new EhllapiExcp("RowToPS failed. Incorrect presentation space ID.");
      else if (rv == 9999)
        throw new EhllapiExcp("RowToPS failed. Character 2 is not P or R.");

      return new DisplayLocation((int)lgth, (int)rc);
    }

    public static DisplayLocation QueryCursorLocation( string InSessId )
    {
      StringBuilder sbKeys = new StringBuilder(10);
      UInt32 rc = 0;
      UInt32 funcNx = Ehllapier.InputCodes.QueryCursorLocation;
      UInt32 lgth = 0;
      UInt32 rv = PcsDll.hllapi(out funcNx, sbKeys, out lgth, out rc);
      if ((rc != 0) || (rv != 0))
        throw new EhllapiExcp(
          "QueryCursorLocation failed.");

      // cursor location is stored as an int16 in the first two bytes of lgth.
      byte[] bits = BitConverter.GetBytes(lgth);
      UInt32 psPos = BitConverter.ToUInt16(bits, 0);
      DisplayLocation loc = PS_ToDisplayLocation(InSessId, psPos); 

      return loc;
    }

    public static FieldAttribute QueryFieldAttribute( 
      string InSessId, DisplayLocation InLoc )
    {
      UInt32 psLoc = DisplayLocationToPS(InSessId, InLoc);

      byte[] buf = new byte[10];
      UInt32 rc = psLoc;
      UInt32 funcNx = Ehllapier.InputCodes.QueryFieldAttribute;
      UInt32 bufLx = 0;
      UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, buf, out bufLx, out rc);

      if ( rc == 7 )
        throw new EhllapiExcp(
          "QueryFieldAttribute failed. Position not valid. ") ;
      else if (rc != 0)
        throw new EhllapiExcp(
          "QueryFieldAttribute failed. " +
          "  rc:" + rc.ToString());

      // the field attribute bits are returned in the bufLx argument. 
      byte[] faBits = BitConverter.GetBytes(bufLx);
      byte faByte = faBits[0];

      // crack the return values.
      FieldAttribute fldAttr = new FieldAttribute( faByte ) ;

      return fldAttr ;
    }

    public static SessionAttributes QuerySessionStatus(string InSessId)
    {

      string s1 = InSessId.PadRight(20, ' ');
      byte[] buf = Encoding.ASCII.GetBytes(s1);
      UInt32 rc = 0;
      UInt32 funcNx = Ehllapier.InputCodes.QuerySessionStatus;
      UInt32 bufLx = (uint)buf.Length;
      UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, buf, out bufLx, out rc);

      if (rc != 0)
        throw new EhllapiExcp(
          "QuerySessionStatus failed. " +
          "  rc:" + rc.ToString());

      // crack the return values.
      var sessStatus = new SessionAttributes();
      sessStatus.SessId = Encoding.ASCII.GetString(buf, 0, 1);
      sessStatus.LongName = Encoding.ASCII.GetString(buf, 4, 8);
      sessStatus.SessionType = Encoding.ASCII.GetString(buf, 12, 1);

      if ((buf[13] & 0x80) != 0)
        sessStatus.ExtendedEab = true;

      if ((buf[13] & 0x40) != 0)
        sessStatus.SupportsProgrammedSymbols = true;

      sessStatus.NumberOfRows = BitConverter.ToInt16(buf, 14);
      sessStatus.NumberOfColumns = BitConverter.ToInt16(buf, 16);
      sessStatus.HostCodePage = BitConverter.ToInt16(buf, 18);

      return sessStatus;
    }

    public static void SendKeys(string InSessId, string InKeys)
    {
      int loopCx = 0;
      StringBuilder sbKeys = null;
      while (true)
      {
        ++loopCx;
        sbKeys = new StringBuilder(InKeys.Length);
        sbKeys.Append(InKeys);
        UInt32 rc = 0;
        UInt32 f = Ehllapier.InputCodes.HA_SENDKEY;
        UInt32 l = (UInt32)sbKeys.Length;
        UInt32 rv = PcsDll.hllapi(out f, sbKeys, out l, out rc);

        // 5 = input to the targer session was inhibited or rejected. all of
        //     the keystrokes could not be sent. 
        if ((rc == 5) && (loopCx < 5))
        {
          Thread.Sleep(1);
          continue;
        }

        else if ((rc != 0) || (rv != 0))
          throw new EhllapiExcp("SendKeys failed. Session " + InSessId + " failed");
        else
          break;
      }
    }

    // ------------------------------- DllImport declares ----------------------------
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    // For Windows Mobile, replace user32.dll with coredll.dll 
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    public static void SetForegroundWindow( string SessId)
    {
      string windowName = "Session " + SessId + " - [24 x 80]";
      System.IntPtr hWnd = FindWindow(null, windowName);
      if (hWnd.ToInt64() != 0)
      {
        SetForegroundWindow(hWnd);
      }
    }

    public static void SetSessionParameters( string InParms)
    {
      Byte[] fac2Bytes = Encoding.ASCII.GetBytes(InParms);
      UInt32 funcNx = Ehllapier.InputCodes.SetSessionParameters;
      UInt32 lgth = (UInt32)fac2Bytes.Length;
      UInt32 rc = 0;
      UInt32 rv = PcsDll.hllapi_Buffer(out funcNx, fac2Bytes, out lgth, out rc);
      if (rv != 0)
        throw new EhllapiExcp("SetSessionParameters failed.");
    }

    /// <summary>
    /// Start close intercept processing of a Session ID. 
    /// Returns the TaskId of the started session. Ehllapi sends a message to the
    /// message loop of the specified hWnd when session close is clicked. The returned
    /// TaskId is passed along in the session close message.
    /// </summary>
    /// <param name="SessId"></param>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    public static int StartCloseIntercept(string SessId, IntPtr hRequestWnd)
    {
      byte[] buf = new byte[12];
      byte[] asciiBuf = Encoding.ASCII.GetBytes(SessId + "M");

      // fill the buffer to pass to the api. 
      for (int ix = 0; ix < buf.Length; ++ix)
      {
        buf[ix] = 0;
      }
      buf[0] = asciiBuf[0];
      buf[4] = asciiBuf[1];

      // store the hWnd of the ehllapi client in the buffer. Position 8.
      // Ehllapi uses this hWnd to know which message loop to send the registered PCSHLL
      // message to.
      var buf4 = BitConverter.GetBytes(hRequestWnd.ToInt32());
      Buffer.BlockCopy(buf4, 0, buf, 8, 4);

      UInt32 rc = 0 ;
      UInt32 funcNx = 41 ;
      UInt32 bufLx = (uint) buf.Length ;
      var rv = PcsDll.hllapi_Buffer(out funcNx, buf, out bufLx, out rc);

      if (rv != 0)
        throw new ApplicationException("StartCloseIntercept failed. SessId:" + SessId +
          " rv:" + rv.ToString());

      int taskId = BitConverter.ToInt16(buf, 8);
      return taskId;
    }

    public static UInt32 StopCloseIntercept(string SessId)
    {
      byte[] buf = new byte[4];
      byte[] asciiBuf = Encoding.ASCII.GetBytes(SessId);

      for (int ix = 0; ix < buf.Length; ++ix)
      {
        buf[ix] = 0;
      }
      buf[0] = asciiBuf[0];

      UInt32 rc = 0;
      UInt32 funcNx = 43;
      UInt32 bufLx = (uint)buf.Length;
      var rv = PcsDll.hllapi_Buffer(out funcNx, buf, out bufLx, out rc);

      return rv;
    }

    public static int StartHostNotification(string SessId, IntPtr hWnd)
    {
      byte[] buf = new byte[16];
      byte[] asciiBuf = Encoding.ASCII.GetBytes(SessId + 'M' + 'O');

      for (int ix = 0; ix < buf.Length; ++ix)
      {
        buf[ix] = 0;
      }
      buf[0] = asciiBuf[0];
      buf[4] = asciiBuf[1];

      // store the hWnd in the buffer. Position 8.
      var buf4 = BitConverter.GetBytes(hWnd.ToInt32());
      Buffer.BlockCopy(buf4, 0, buf, 8, 4);

      buf[12] = asciiBuf[2];

      UInt32 rc = 0;
      UInt32 funcNx = 23;
      UInt32 bufLx = (uint)buf.Length;
      var rv = PcsDll.hllapi_Buffer(out funcNx, buf, out bufLx, out rc);

      if (rv != 0)
        throw new ApplicationException("StartHostNotification failed. SessId:" + SessId +
          " rv:" + rv.ToString());

      int taskId = BitConverter.ToInt16(buf, 8);
      return taskId;
    }

    public static void StopHostNotification(string SessId)
    {
      byte[] buf = new byte[4];
      byte[] asciiBuf = Encoding.ASCII.GetBytes(SessId);

      for (int ix = 0; ix < buf.Length; ++ix)
      {
        buf[ix] = 0;
      }
      buf[0] = asciiBuf[0];

      UInt32 rc = 0;
      UInt32 funcNx = 25;
      UInt32 bufLx = (uint)buf.Length;
      var rv = PcsDll.hllapi_Buffer(out funcNx, buf, out bufLx, out rc);

      if (rv != 0)
        throw new ApplicationException("StopHostNotification failed. SessId:" + SessId +
          " rv:" + rv);
    }

    public static void Wait(string InSessId)
    {
      StringBuilder Data = new StringBuilder(0);
      UInt32 rc = 0;
      UInt32 f = Ehllapier.InputCodes.HA_WAIT;
      UInt32 l = 0;
      UInt32 rv = PcsDll.hllapi(out f, Data, out l, out rc);
      if ((rc != 0) || (rv != 0))
        throw new EhllapiExcp(
          "Session wait failed. " +
          " rc: " + rc.ToString() +
          " SessId: " + InSessId);
    }
  }
}

