using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Text.Enums
{

  // --------------------------- WordCompositeCode ----------------------------
  // classifies how words are grouped in the StmtParser first pass.
  // words are either a collection of paren enclosed words, a sequence of words
  // in sentence form, or a standalone word.
  public enum WordCompositeCode
  {
    Atom, Braced, Sentence, General, None, Any
  }

  public static class WordCompositeCodeExt
  {

    /// <summary>
    /// test if the enum value is equal any of the compare to enum values.
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="Values"></param>
    /// <returns></returns>
    public static bool EqualAny(this WordCompositeCode CompCode, params WordCompositeCode[] Values)
    {
      bool rc = false;
      foreach (var vlu in Values)
      {
        if (vlu == CompCode)
        {
          rc = true;
          break;
        }
      }
      return rc;
    }

    public static bool IsAtomOrBraced(this WordCompositeCode? Code)
    {
      if (Code == null)
        return false;
      else if (Code.Value == WordCompositeCode.Atom)
        return true;
      else if (Code.Value == WordCompositeCode.Braced)
        return true;
      else
        return false;
    }
  }
}


