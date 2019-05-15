using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Sql
{

  // -------------------------- TableNotFound  ---------------------
  public class TableNotFound : ApplicationException
  {

    public TableNotFound(string InTableName)
      : base("Table " + InTableName + " is not found")
    {
    }
  }
}
