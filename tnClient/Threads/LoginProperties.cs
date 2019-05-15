using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tnClient.Threads
{
  public class LoginProperties
  {
    string mLoginSystem = "";
    string mLoginUser = "";
    string mLoginPass = "";
    SystemBrand mBrand = SystemBrand.None;

    public LoginProperties()
    {
    }

    /// <summary>
    /// Brand ( AIX, VIOS, AMM, ... of the system to login to.
    /// </summary>
    public SystemBrand Brand
    {
      get { return mBrand; }
      set { mBrand = value; }
    }

    public string LoginPass
    {
      get { return mLoginPass; }
      set
      {
        if (value == null)
          mLoginPass = "";
        else
          mLoginPass = value;
      }
    }

    public string LoginSystem
    {
      get { return mLoginSystem; }
      set
      {
        if (value == null)
          mLoginSystem = "";
        else
          mLoginSystem = value;
      }
    }

    public string LoginUser
    {
      get { return mLoginUser; }
      set
      {
        if (value == null)
          mLoginUser = "";
        else
          mLoginUser = value;

      }
    }

  }

}
