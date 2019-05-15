using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Ehllapi
{
  public class SessionAttributes
  {
    string mSessId;
    string mLongName;
    string mSessionType;
    bool mExtendedEab = false;
    bool mSupportsProgrammedSymbols = false;
    int mNumberOfRows = 0;
    int mNumberOfColumns = 0;
    int mHostCodePage = 0;

    public SessionAttributes()
    {
    }

    public bool ExtendedEab
    {
      get { return mExtendedEab; }
      set { mExtendedEab = value; }
    }

    public int HostCodePage
    {
      get { return mHostCodePage; }
      set { mHostCodePage = value; }
    }

    public string LongName
    {
      get { return mLongName; }
      set
      { 
        mLongName = value.TrimEnd( new char[] {' ', '\0'}) ;
      }
    }

    public int NumberOfColumns
    {
      get { return mNumberOfColumns; }
      set { mNumberOfColumns = value; }
    }

    public int NumberOfRows
    {
      get { return mNumberOfRows; }
      set { mNumberOfRows = value; }
    }

    public string SessId
    {
      get { return mSessId; }
      set { mSessId = value; }
    }

    public string SessionType
    {
      get { return mSessionType; }
      set { mSessionType = value; }
    }

    public bool SupportsProgrammedSymbols
    {
      get { return mSupportsProgrammedSymbols; }
      set { mSupportsProgrammedSymbols = value; }
    }

  }
}
