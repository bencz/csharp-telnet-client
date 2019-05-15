using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using AutoCoder.Ext.System;
using AutoCoder.Text;
using AutoCoder.Microsoft.Win32;

namespace AutoCoder.Ext.Microsoft.Win32
{
  public static class RegistryKeyExt
  {

    public static bool KeyNameCouldBeGuid(this string KeyName)
    {
      if (KeyName.IsNullOrEmpty())
        return false;
      else if (KeyName.Length != 38)
        return false;
      else if (KeyName[0] != '{')
        return false;
      else if (KeyName[37] != '}')
        return false;
      else
        return true ;
    }

    /// <summary>
    /// return the default value of the registry key. The default value is the value
    /// with a name that is empty.
    /// </summary>
    /// <param name="RegKey"></param>
    /// <returns></returns>
    public static object DefaultValue(this RegistryKey RegKey)
    {
      object defaultValue = null;
      var valueNames = RegKey.GetValueNames();
      foreach (var vluName in valueNames)
      {
        if (vluName.IsNullOrEmpty())
        {
          defaultValue = RegKey.GetValue(vluName);
          break;
        }
      }

      return defaultValue;
    }

    /// <summary>
    /// determine if this registrykey contains a value with a name that is empty, that
    /// is the default value of the registry key.
    /// </summary>
    /// <param name="RegKey"></param>
    /// <returns></returns>
    public static bool HasDefaultValue(this RegistryKey RegKey)
    {
      var valueNames = RegKey.GetValueNames();
      if (valueNames.Length == 0)
        return false;
      else
      {
        foreach (var vluName in valueNames)
        {
          if (vluName.IsNullOrEmpty())
            return true;
        }
        return false;
      }
    }

    /// <summary>
    /// This is a root registry key, the registry hive. The registry path name does not
    /// contain a "\".
    /// </summary>
    /// <param name="RegKey"></param>
    /// <returns></returns>
    public static bool IsRegistryHive(this RegistryKey RegKey)
    {
      var keyName = RegKey.Name;
      if (keyName.IndexOf('\\') == -1)
        return true;
      else
        return false;
    }

    public static RegistryKey OpenBaseKey(string BaseKeyName)
    {
      var rhEnum = BaseKeyName.ParseAsRegistryHive();
      return RegistryKey.OpenBaseKey(rhEnum, RegistryView.Default);
    }

    public static RegistryKey OpenRegistryKey(string KeyPath)
    {
      RegistryKey subKey = null;
      var rootName = RegistryKeyExt.GetRoot(KeyPath);
      var subKeyPath = RegistryKeyExt.GetPathAfterRoot(KeyPath);

      if (rootName.IsNullOrEmpty())
        subKey = RegistryKeyExt.OpenBaseKey(KeyPath) ;
      else
      {
        using (var bk = RegistryKeyExt.OpenBaseKey(rootName))
        {
          subKey = bk.OpenSubKey(subKeyPath);
        }
      }

      return subKey;
    }

    /// <summary>
    /// Same as Path.Combine, only the method does not throw an exception if either part
    /// contains invalid file path characters.
    /// </summary>
    /// <param name="Directory"></param>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public static string Combine(string Directory, string FileName)
    {
      if (Directory.IsNullOrEmpty() == true)
        return FileName;
      else
        return Directory + '\\' + FileName;
    }


    /// <summary>
    /// returns a sequence of registry key paths to file extensions in the registry.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> FileExtensionRegistryKeys()
    {
      using (var cr =
        RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default))
      {
        foreach (var keyName in cr.GetSubKeyNames())
        {
          if ((keyName.Length > 0) && (keyName[0] == '.'))
          {
            var extPath = RegistryKeyExt.Combine(cr.Name, keyName);
            yield return extPath;
          }
        }
      }
      yield break;
    }

    public static string GetFileName(string RegKeyName)
    {
      var found = Scanner.ScanReverseEqual(RegKeyName, '\\');
      if (found.ResultPos == -1)
        return RegKeyName;
      else
      {
        var fileName = RegKeyName.Substring(found.ResultPos + 1);
        return fileName;
      }
    }

    /// <summary>
    /// return the portion of the input path that runs from the start to the directory
    /// name that preceeds the last name in the path.
    /// </summary>
    /// <param name="RegKeyName"></param>
    /// <returns></returns>
    public static string GetDirectoryName( string RegKeyName )
    {

      var found = Scanner.ScanReverseEqual(RegKeyName, '\\');
      if (found.ResultPos == -1)
        return "";
      else
      {
        var lx = found.ResultPos;
        return RegKeyName.Substring(0, lx);
      }
    }

    /// <summary>
    /// return the name, value and kind of the specified value name.
    /// </summary>
    /// <param name="RegKey"></param>
    /// <param name="ValueName"></param>
    /// <returns></returns>
    public static RegistryKeyValue GetNamedValue(this RegistryKey RegKey, string ValueName)
    {
      var valueNameLower = ValueName.ToLower();
      foreach (var vName in RegKey.GetValueNames())
      {
        var vNameLower = vName.ToLower();
        if (vNameLower == valueNameLower)
        {
          var rkv = new RegistryKeyValue()
          {
            ValueName = vName,
            ValueValue = RegKey.GetValue(vName),
            ValueKind = RegKey.GetValueKind(vName)
          };
          return rkv;
        }
      }

      return null;
    }

    public static RegistryKeyValue GetNamedValue(
      this RegistryKey RegKey, string ValueName,
      string[] SelectValue )
    {
      var valueNameLower = ValueName.ToLower();
      foreach (var vName in RegKey.GetValueNames())
      {
        var vNameLower = vName.ToLower();
        if (vNameLower == valueNameLower)
        {

          // skip if not one of the SelectValue values.
          bool selectMatch = false;
          var vlu = RegKey.GetValue(vName);
          if (vlu is string)
          {
            foreach (var sv in SelectValue)
            {
              if (sv.ToLower() == (vlu as string).ToLower())
              {
                selectMatch = true;
                break;
              }
            }
          }
          if (selectMatch == true)
          {
            var rkv = new RegistryKeyValue()
            {
              ValueName = vName,
              ValueValue = RegKey.GetValue(vName),
              ValueKind = RegKey.GetValueKind(vName)
            };
            return rkv;
          }
        }
      }

      return null;
    }

    public static IEnumerable<RegistryKeyValue> GetValues(this RegistryKey RegKey)
    {
      foreach (var vName in RegKey.GetValueNames())
      {
        var rkv = new RegistryKeyValue()
        {
          ValueName = vName,
          ValueValue = RegKey.GetValue(vName),
          ValueKind = RegKey.GetValueKind(vName)
        };
        yield return rkv;
      }
      yield break;
    }

    /// <summary>
    /// returns a recursive sequence of the sub keys of a registry key.
    /// </summary>
    /// <param name="ScopeKeyPath"></param>
    /// <returns></returns>
    public static IEnumerable<string> RegistryKeysDeep(string ScopeKeyPath)
    {
      var regKey = RegistryKeyExt.OpenRegistryKey(ScopeKeyPath);
      if (regKey == null)
        yield break;
      else
      {
        foreach (var subKeyName in regKey.GetSubKeyNames())
        {
          // first, return the sub key.
          var subKeyPath = RegistryKeyExt.Combine(regKey.Name, subKeyName);
          yield return subKeyPath;

          // next, return the sub keys of the sub key.
          foreach (var keyPath in RegistryKeyExt.RegistryKeysDeep(subKeyPath))
          {
            yield return keyPath;
          }
        }
        regKey.Close();
        yield break;
      }
    }

    public static Tuple<string, string> SplitLeft(string RegKeyPath)
    {
      string leftPart = null;
      string remPart = null;
      var fx = RegKeyPath.IndexOf('\\');
      if (fx == -1)
      {
        leftPart = RegKeyPath;
      }
      else
      {
        int remBx = fx + 1 ;
        int remLx = RegKeyPath.Length - remBx ;
        leftPart = RegKeyPath.Substring(0, fx);
        if (remLx > 0)
          remPart = RegKeyPath.Substring(remBx);
      }
      return new Tuple<string, string>(leftPart, remPart);
    }

    /// <summary>
    /// return the root directory from the path.
    /// </summary>
    /// <param name="RegKeyPath"></param>
    /// <returns></returns>
    public static string GetRoot(string RegKeyPath)
    {
      var fx = RegKeyPath.IndexOf('\\');
      if (fx == -1)
        return "";
      else
      {
        return RegKeyPath.Substring(0, fx);
      }
    }

    /// <summary>
    /// return the part of the path that follows the root directory of the path.
    /// </summary>
    /// <param name="RegKeyName"></param>
    /// <returns></returns>
    public static string GetPathAfterRoot(string RegKeyName)
    {
      var fx = RegKeyName.IndexOf('\\');
      if (fx == -1)
        return RegKeyName;
      else
      {
        int bx = fx + 1 ;
        int lx = RegKeyName.Length - bx;
        if (lx > 0)
          return RegKeyName.Substring(bx);
        else
          return "";
      }
    }

  }
}
