using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.ComponentModel;
using AutoCoder.Ext.Microsoft.Win32;
using AutoCoder.Core.Enums;
using System.Collections.ObjectModel;
using AutoCoder.Ext.System;
using Microsoft.Win32;

namespace AutoCoder.Microsoft.Win32
{
  /// <summary>
  /// a RegistryKeyFragment is a registry key path packaged to be bound to a TreeView,
  /// where each level of the treeview contains a single, expanded item. Under the last
  /// TreeView item, all of the values of that bottom treeview item are listed.
  /// </summary>
  public class RegistryKeyFragment : IDisposable
  {

    private readonly ObservableCollection<FragmentKeyItem> _KeyPart
      = new ObservableCollection<FragmentKeyItem>();
    public ObservableCollection<FragmentKeyItem> KeyPart
    {
      get { return _KeyPart; }
    }

    FragmentKeyItem _KeyPartRoot;
    public FragmentKeyItem KeyPartRoot
    {
      get
      {
        return _KeyPartRoot;
      }
      set 
      {
        if (_KeyPartRoot == null)
        {
          _KeyPartRoot = value;
          this.KeyPart.Add(this.KeyPartRoot);
        }
        else
          throw new ApplicationException("KeyPartRoot is already set.");
      }
    }

    public RegistryKeyFragment(string KeyPath)
    {
      string cummPath = null;
      string keyPath = KeyPath;
      while (keyPath.IsNullOrEmpty() == false)
      {
        var rv = RegistryKeyExt.SplitLeft(keyPath);
        var rootName = rv.Item1;
        keyPath = rv.Item2;

        cummPath = RegistryKeyExt.Combine(cummPath, rootName);

        var item = new FragmentKeyItem()
        {
          KeyPath = cummPath
        };

        if (this.KeyPartRoot == null)
          this.KeyPartRoot = item;
        else
        {
          var lastChild = this.KeyPartRoot.LastChildDeep;
          lastChild.Children.Add(item);
        }
      }

      // from the full key of the fragment, list all the sub keys and values.
      if (this.KeyPartRoot != null)
      {
        var lastChild = this.KeyPartRoot.LastChildDeep;
        using (var rk = RegistryKeyExt.OpenRegistryKey(KeyPath))
        {
          AddSubKeys(rk, lastChild);
          AddValues(rk, lastChild);
        }
      }
    }

    private void AddValues(RegistryKey RegKey, FragmentKeyItem Child)
    {
      foreach (var rkv in RegKey.GetValues())
      {
        var item = new FragmentValueItem()
        {
          KeyName = rkv.ShowValueName + " " + rkv.ValueValue,
          KeyValue = rkv
        };

        Child.Children.Add(item);
      }
    }

    private void AddSubKeys(RegistryKey RegKey, FragmentKeyItem Child)
    {
      foreach (var keyName in RegKey.GetSubKeyNames())
      {
        using (var subKey = RegKey.OpenSubKey(keyName))
        {
          var keyItem = new FragmentKeyItem()
          {
            KeyPath = subKey.Name
          };
          Child.Children.Add(keyItem);

          AddSubKeys(subKey, keyItem);
          AddValues(subKey, keyItem);
        }
      }
    }

    ~RegistryKeyFragment()
    {
      Dispose();
    }

    public void Dispose()
    {
    }

    public class FragmentValueItem : HierarchicalModelBase
    {

      public string KeyName
      { get; set; }

      public RegistryKeyValue KeyValue
      { get; set; }

      protected override void FillExpandableMarker()
      {
        return;
      }

      protected override void FillChildren()
      {
        return;
      }

      public override bool CanExpand
      {
        get { return false; }
      }
    }

    public class FragmentKeyItem : HierarchicalModelBase
    {


      ObservableCollection<FragmentKeyItem> _RootNode;
      public ObservableCollection<FragmentKeyItem> RootNode
      {
        get
        {
          if (_RootNode == null)
          {
            _RootNode = new ObservableCollection<FragmentKeyItem>();
            this.RootNode.Add(this);
          }
          return _RootNode;
        }
      }

      /// <summary>
      /// The full registry path to this registry key.
      /// </summary>
      public string KeyPath
      { get; set; }

      /// <summary>
      /// the simple name of the key. The "FileName" of the KeyPath.
      /// </summary>
      public string KeyName
      {
        get
        {
          return RegistryKeyExt.GetFileName(this.KeyPath);
        }
      }

      /// <summary>
      /// the FragmentKeyItem has a Children collection. Recursively drill down the last 
      /// child of the item until a child with no children. That item is the
      /// LastChildDeep.
      /// </summary>
      public FragmentKeyItem LastChildDeep
      {
        get
        {
          if (this.Children.Count() == 0)
            return this;
          else
          {
            var lastIx = this.Children.Count() - 1;
            var lastChild = this.Children[lastIx] as FragmentKeyItem ;
            var lastChildDeep = lastChild.LastChildDeep ;
            return lastChildDeep ;
          }
        }
      }

      public override bool CanExpand
      {
        get { return false; }
      }

      /// <summary>
      /// FragmentKeyItems do not contain expandable markers.
      /// </summary>
      protected override void FillExpandableMarker()
      {
        return;

#if skip
        using (var regKey = RegistryKeyExt.OpenRegistryKey(this.KeyPath))
        {
          if (regKey != null)
          {
            if (regKey.GetSubKeyNames().Count() > 0)
            {
              var model = new FragmentKeyItem()
              {
                Parent = this,
                HasChildrenMarker = CollectionMarker.HasChildrenMarker
              };
              Children.Add(model);
            }
          }
        }
#endif
      }

      protected override void FillChildren()
      {
        Children.Clear();

        using (var regKey = RegistryKeyExt.OpenRegistryKey(this.KeyPath))
        {

          bool isRoot = regKey.IsRegistryHive();

          foreach (var keyName in regKey.GetSubKeyNames())
          {

            using (var subKey = regKey.OpenSubKey(keyName))
            {
              var model = new FragmentKeyItem()
              {
                Parent = this,
                HasChildrenMarker = CollectionMarker.HasChildrenMarker
              };
              Children.Add(model);
            }
          }
        }
      }
    }
  }
}
