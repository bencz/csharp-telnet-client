using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Scan;

namespace AutoCoder.Ftp
{

  // ------------------------ FtpDirEntry ------------------------------------
  public class FtpDirEntry
  {
    AcFileType mEntryType;
    string mEntryName;
    DateTime mChgDateTime;
    string mTextLine;
    Int64 mFileSize = 0;

    public FtpDirEntry(string InLine)
    {
      TextLine = InLine;
      Parse( TextLine );
    }

    public DateTime ChgDateTime
    {
      get { return mChgDateTime; }
      set { mChgDateTime = value; }
    }

    public string EntryName
    {
      get
      {
        return Stringer.GetNonNull(mEntryName);
      }
      set { mEntryName = value; }
    }

    public AcFileType EntryType
    {
      get { return mEntryType ; }
      set { mEntryType = value ; }
    }

    public bool IsEmpty
    {
      get { return ( EntryName.Length == 0 ) ; }
    }

    public Int64 FileSize
    {
      get { return mFileSize; }
      set { mFileSize = value; }
    }

    public string TextLine
    {
      get { return Stringer.GetNonNull( mTextLine ) ; }
      set
      {
        char[] ws = { '\r', ' ' };
        mTextLine = value.TrimEnd(ws);
      }
    }

    private void Parse( string Line )
    {
      // split on space
      string[] words = 
        Line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      // at least 5 words. And 5th word is *STMF.
      if ((words.Length >= 5) && (words[4] == "*STMF"))
      {
        string chSize = words[1];
        FileSize = Int64.Parse(chSize);
        int fx = Line.IndexOf("*STMF");
        int bx = fx + 5;
        EntryName = Line.Substring(bx).Trim(new char[] { ' ', '\r' });
        EntryType = AcFileType.File;
      }

      // at least 5 words. And 5th word is *DIR.
      else if ((words.Length >= 5) && (words[4] == "*DIR"))
      {
        string chSize = words[1];
        FileSize = Int64.Parse(chSize);
        int fx = Line.IndexOf("*DIR");
        int bx = fx + 4;
        EntryName = Line.Substring(bx).Trim(new char[] { ' ', '\r', '/' });
        EntryType = AcFileType.Folder;
      }

      // 9 words.
      else if (words.Length == 9)
      {
        string chSize = words[4];
        FileSize = Int64.Parse(chSize);
        EntryName = words[8];
        EntryType = AcFileType.File;
      }

      // the line has 4 words. assume in form "date time <DIR>/fileSize name".
      else if (words.Length == 4)
      {
        if (words[2] == "<DIR>")
        {
          EntryName = words[3];
          EntryType = AcFileType.Folder;  
          }
        else
        {
          string chSize = words[4];
          FileSize = Int64.Parse(chSize);
          EntryName = words[8];
          EntryType = AcFileType.File;
        }

        DateTime dt = DateTime.Parse(words[0] + " " + words[1]);
        ChgDateTime = dt;
      }
      else
      {
        if (Line.Length == 0)
        {
          EntryType = AcFileType.None;
          EntryName = null;
          throw new ApplicationException( "FTP directory text line is empty" ) ;
        }
        else if (mTextLine.EndsWith("/"))
        {
          int Lx = mTextLine.Length - 1;
          EntryName = mTextLine.Substring(0, Lx);
          EntryType = AcFileType.Folder;
        }
        else
        {
          EntryName = mTextLine;
          EntryType = AcFileType.File;
        }
      }
    }

    string[] Parse_CrackWords(string InLine)
    {
      List<string> words = new List<string>();
      TextTraits traits = new TextTraits();
      traits.DividerPatterns.Clear( ) ;
      traits.OpenContentBracedPatterns.Clear();
      traits.OpenNamedBracedPatterns.Clear();

      // the dir entry name can contain spaces. This traits object
      // is used
      TextTraits entryNameTraits = new TextTraits(traits);
      entryNameTraits.WhitespacePatterns.Replace("\t", Text.Enums.DelimClassification.Whitespace) ;

      WordCursor csr = null;
      csr = Scanner.PositionBeginWord(InLine, traits);
      while (true)
      {
        // the 4th word is the file/dir name. This word has a diff char set,
        // it can have a space in the name.
        if (words.Count == 3)
          csr.TextTraits = entryNameTraits;
        else
          csr.TextTraits = traits;

        csr = Scanner.ScanNextWord(InLine, csr);
        if (csr.IsEndOfString == true)
          break;

        if (words.Count == 3)
          words.Add(csr.Word.Value.Trim());
        else
          words.Add(csr.Word.Value);
      }

      return words.ToArray();
    }


  } // end class FtpDirEntry
}
