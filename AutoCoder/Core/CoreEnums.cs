
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AutoCoder.Core
{

  public enum TypeEnum { Bool, Size, String, Decimal, Int, Int32, Int64, DateTime, Void, none } ;
  public enum AcRelativePosition { Begin, Before, After, End, At, None } ;
  public enum AcReturnCode { Ok, Exception, None } ;
  public enum AcFileType { File, Directory, Folder, None } ;

  // instructs the method that returns Next in a tree structure to assert that the
  // next node must be a child or sibling of the current node.
  public enum AssertNextChildNode { NoChild, HasChild, None } ;
  public enum AssertNextNodeRelative { IsChild, IsSibling, None } ; 
  public enum AssertNextSiblingNode { NoNextSibling, HasNextSibling, None } ;

  // case of characters in a string.
  public enum CharCase { Upper, Lower, Mixed, None, Any } ;

  public enum CommDirection { Send, Receive, None } ;

  public enum WorkToDo { Add, Change, Delete, Display, Copy, Print, None } ;
  public enum ActionTaken { Cancel, Update, OK, Insert, None } ;

  public enum CheckResult { Found, NotFound, None } ;
  public enum FoundResult { Found, NotFound, None } ;
  public enum ErrorResult { Error, OK } ;

  public enum PrintOrientation { Portrait, Landscape, None } ;

  // ------------------- HowUrlEncodeSpaceChar ------------------------
  // how to url encode a space character. Plus means replace space char 
  // with a "+" char. Hex means replace with the hex value ( %20 ), like all other
  // url encoded replacement chars. 
  // Used in the Stringer.UrlEncode method.
  public enum HowUrlEncodeSpaceChar { Plus, Hex, None } ;

  // result of parsing a source line.
  // OK - parsing was successful. What was being parsed is complete.
  public enum LineParseResult { OK, Error, More, None }

  // other possible names: NotFoundTodo, MissingResponse
  public enum NotFoundHandling { Exception, Null, Create, None } ;

  public enum ScannerEndOfLineConsider { Found, ContinueScan } ;
  public enum ScannerWhatFound { PatternChar, PatternString, HardLineBreak, NotFound } ;
  public enum ScannerBracedTreatment { Parts, Whole } ;

  // the compare rule to use when scanning for a pattern.
  public enum ScanCompareRule { Equal, NotEqual, Less, Greater, LessEqual, GreaterEqual, None } ;

  // which edge of a tree node which contains sub nodes
  public enum WhichEdge { LeadEdge, TrailEdge, None, Any } ;

  // how a rule eval results should be finally processed. Either invert the result or 
  // leave it as is. See CursorMatchPattern for usage of this enum.
  public enum RulePolarity { Positive, Negative, None } ;

  // ----------------------------- WingdingChars ---------------------------
  // the integer value of select WingdingChars. An example use of a Wingdings char is
  // to set the font of a button to Wingdings, then set the button text to one of the
  // symbol characters.
  //    Font f = new Font("Wingdings", 10, FontStyle.Regular);
  //    char ch1;
  //    butUpArrow = new Button();
  //    butUpArrow.Font = f;
  //    ch1 = (char)WingdingChars.UpArrow;
  //    butUpArrow.Text = ch1.ToString();
  //    butUpArrow.Width = Font.Height * 2;
  public enum WingdingChars
  {
    LeftArrow = 231, RightArrow = 232, UpArrow = 233, DownArrow = 234,
    BallotX = 251, CheckMark = 252, None = 0
  } ;

  public enum WingdingChars2
  {
    DeleteX = 79, CheckMark = 80,
    None = 0
  } ;

  public enum WingdingChars3
  {
    UpArrow = 163, DownArrow = 164,
    None = 0
  } ;

  // common form event delegates
  public delegate void delButtonClicked( Control InControl, string InButtonName ) ;
  public delegate void delCancelButtonClicked( Control InControl );
  public delegate void delRunButtonClicked( Control InControl );
  public delegate void delSaveButtonClicked( Control InControl );

  // used to call the Clipboard.GetClipboardData method on STA thread.
  public delegate object delGetClipboardData( string InFormat ) ;
  
  // Delegate used to signal that something in a list has changed.  Used, for example,
  // by a control to know when to repaint itself because the associated list of content
  // data has changed.
  public delegate void delListChanged( object InList ) ;

  // thread has ended. A running thread calls this callback right before it exits its
  // entry procedure.
  public delegate void delThreadEnded( string InThreadName ) ;

  // write a text message to an event log.

  // SubscriberData is optional. Can be null.
  // SubscriberData is an object the subscriber provides to the service. When the 
  // service signals the event, it passes back that subscriber data object. The 
  // event handler of the subscriber uses the SubscriberData to provide context to
  // the signaled event. 
  public delegate void delWriteEventLog( 
  object SubscriberData,
  string InText, string InActivity ) ;

  public static class CoreEnums
  {
    public static string TypeEnumToString(TypeEnum InEnum)
    {
      switch (InEnum)
      {
        case TypeEnum.Bool:
          return "bool";
        case TypeEnum.DateTime:
          return "datetime";
        case TypeEnum.Decimal:
          return "decimal";
        case TypeEnum.String:
          return "string";
        case TypeEnum.Size:
          return "Size";
        case TypeEnum.Int:
          return "int";
        case TypeEnum.Int32:
          return "Int32";
        case TypeEnum.Int64:
          return "Int64";
        case TypeEnum.Void:
          return "void";
        default:
          throw new Exception("unexpected TypeEnum " + InEnum.ToString());
      }
    }

    /// <summary>
    /// return the inverse of a scan compare rule. See ScanPattern class. Patterns can be
    /// scan for based on the specified CompareRule. ScanPatterns can also be scanned "not"
    /// for, where the inverse of the CompareRule is evaluated.
    /// </summary>
    /// <param name="InCompareRule"></param>
    /// <returns></returns>
    public static ScanCompareRule CalcInverseRule(ScanCompareRule InCompareRule)
    {
      switch (InCompareRule)
      {
        case ScanCompareRule.Equal:
          return ScanCompareRule.NotEqual;
        case ScanCompareRule.NotEqual:
          return ScanCompareRule.Equal;
        case ScanCompareRule.Greater:
          return ScanCompareRule.LessEqual;
        case ScanCompareRule.GreaterEqual:
          return ScanCompareRule.Less;
        case ScanCompareRule.Less:
          return ScanCompareRule.GreaterEqual;
        case ScanCompareRule.LessEqual:
          return ScanCompareRule.Greater;
        case ScanCompareRule.None:
          return ScanCompareRule.None;
        default:
          throw new ApplicationException("unexpected ScanCompareRule"); 
      }
    }

  }
}