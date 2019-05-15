using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace AutoCoder.Ext.Microsoft.Win32
{
  public static class RegistryHiveExt
  {
    public static RegistryHive ParseAsRegistryHive(this string HiveName)
    {
      if (HiveName == "HKEY_CLASSES_ROOT")
        return RegistryHive.ClassesRoot;
      else if (HiveName == "HKEY_CURRENT_USER")
        return RegistryHive.CurrentUser;
      else if (HiveName == "HKEY_LOCAL_MACHINE")
        return RegistryHive.LocalMachine;
      else if (HiveName == "HKEY_USERS")
        return RegistryHive.Users;
      else if (HiveName == "HKEY_PERFORMANCE_DATA")
        return RegistryHive.PerformanceData;
      else if (HiveName == "HKEY_CURRENT_CONFIG")
        return RegistryHive.CurrentConfig;
      else if (HiveName == "HKEY_DYN_DATA")
        return RegistryHive.DynData;
      else
        throw new ApplicationException("invalid hive name " + HiveName);
    }
  }
}
