using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ehllapi.Enums
{
  public enum SessionStatus
  {
    Started,
    OnLine,
    ApiEnabled,  // remove this
    InterimState
  }

  public static class SessionStatusExt
  {

    public static SessionStatus ToSessionStatus(this uint Value)
    {
      const uint PCS_SESSION_STARTED = 0x01;
      const uint PCS_SESSION_ONLINE = 0x02;
      const uint PCS_SESSION_API_ENABLED = 0x04;
      const uint PCS_SESSION_INTERIM_STATE = 0x08;

      if ((Value & PCS_SESSION_ONLINE) != 0)
        return SessionStatus.OnLine;
      else if ((Value & PCS_SESSION_STARTED) != 0)
        return SessionStatus.Started;
      else
        throw new ApplicationException(
          "unsupported SessionStatus value " + Value.ToString());

      if (Value == PCS_SESSION_STARTED)
        return SessionStatus.Started;
      else if (Value == PCS_SESSION_ONLINE)
        return SessionStatus.OnLine;
      else if (Value == PCS_SESSION_API_ENABLED)
        return SessionStatus.ApiEnabled;
      else if (Value == PCS_SESSION_INTERIM_STATE)
        return SessionStatus.InterimState;
      else
        throw new ApplicationException(
          "unsupported SessionStatus value " + Value.ToString());
    }
  }
}

