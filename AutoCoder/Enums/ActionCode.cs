using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Enums
{
  // would like to seperate enums for the action to perform from the action performed.
  // ActionToPerform, ActionPerformed   WorkToDo  ActionTaken  WorkCode  ActionCode
  public enum ActionCode
  {
    Add, Change, Delete, Display, Copy, Print, Cancel, Apply, OK, Move, None
  };

}
