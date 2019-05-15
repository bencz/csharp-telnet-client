using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Reflection;
using System.Windows;

namespace AutoCoder.Core
{
  /// <summary>
  /// string which contains the type of an object.
  /// </summary>
  public class FrameworkType
  {

    // dictionary used to store and find the Type of a FrameworkType.
    // Used by the GetType method.
    static Dictionary<string, Type> mTypeLookup = 
      new Dictionary<string, Type>();

    string _TypeText = null;
    string mFullTypeName = null;

    // Eenum that contains names of types which have builtin support in this
    // class. Names like bool, decimal, int, etc.
    TypeEnum _TypeEnum;
    
    bool _TypeEnumIsAssigned = false;
    int _ArrayLength = -1;

    public FrameworkType(string InText)
    {
      string typeText = null;
      ParseInternal(out typeText, out _ArrayLength, InText);
      TypeText = typeText;
    }

    public FrameworkType( TypeEnum InTypeEnum )
    {
      _TypeEnum = InTypeEnum ;
      _TypeEnumIsAssigned = true ;
      _ArrayLength = -1;
    }

    public FrameworkType(TypeEnum InTypeEnum, int InArrayLength)
    {
      _TypeEnum = InTypeEnum;
      _TypeEnumIsAssigned = true;
      _ArrayLength = InArrayLength;
    }


    public int ArrayLength
    {
      get { return _ArrayLength; }
      set { _ArrayLength = value; }
    }

    public static FrameworkType Bool
    {
      get { return new FrameworkType("bool"); }
    }

    public static FrameworkType Decimal
    {
      get { return new FrameworkType("decimal"); }
    }

    public string FullTypeName
    {
      get
      {
        if (mFullTypeName == null)
        {
          switch (TypeEnum)
          {
            case TypeEnum.Bool:
              mFullTypeName = "System.Boolean";
              break;
            case TypeEnum.Decimal:
              mFullTypeName = "System.decimal";
              break;
            case TypeEnum.Int:
              mFullTypeName = "System.Int32";
              break;
            case TypeEnum.Int32:
              mFullTypeName = "System.Int32";
              break;
            case TypeEnum.Int64:
              mFullTypeName = "System.Int64";
              break;
            case TypeEnum.Size:
              mFullTypeName = "System.Drawing.Size";
              break;
            case TypeEnum.String:
              mFullTypeName = "System.String";
              break;
            default:
              throw new Exception("Cannot calc FullName of FrameworkType " + TypeText ) ;
          }
        }
        return mFullTypeName;
      }
      set { mFullTypeName = value; }
    }

    /// <summary>
    /// return the Type of this framework type.
    /// </summary>
    /// <returns></returns>
    public Type GetSystemType()
    {
      Type gotType = null;

      // return the type found in TypeLookup static.
      if (mTypeLookup.ContainsKey(FullTypeName) == false)
      {
        Type tp = Type.GetType(FullTypeName);

        // search for the type by its FullName.
        if (tp == null)
          tp = Typer.FindType(FullTypeName);

        if (tp == null)
          throw new Exception("Type of FrameworkType " + TypeText + " is not found");

        mTypeLookup.Add(FullTypeName, tp);
      }
      gotType = mTypeLookup[FullTypeName];

      return gotType;
    }

    public static FrameworkType Int
    {
      get { return new FrameworkType("int"); }
    }

    public static FrameworkType Int32
    {
      get { return new FrameworkType("Int32"); }
    }

    private static void ParseInternal(
      out string OutTypeText, out int OutArrayLength, string InText)
    {
      string typeText = null;
      int arrayLength = -1;

      int ex = 0;
      int lx = 0;
      int bx = InText.IndexOf('[');
      if (bx >= 0)
      {
        ex = InText.IndexOf(']');
        lx = ex - bx - 1;
        if (lx == 0)
          arrayLength = 0;
        else
        {
          string s1 = InText.Substring(bx + 1, lx);
          arrayLength = System.Int32.Parse(s1);
        }
        typeText = InText.Substring(0, bx);
      }
      else
      {
        arrayLength = -1;
        typeText = InText;
      }

      OutTypeText = typeText;
      OutArrayLength = arrayLength;
    }

    public static FrameworkType Size
    {
      get { return new FrameworkType("Size"); }
    }

    public static FrameworkType String
    {
      get { return new FrameworkType("string"); }
    }

    public override string ToString()
    {
      if (_ArrayLength == -1)
        return TypeText;
      else if (_ArrayLength == 0)
        return TypeText + "[]";
      else
        return TypeText + "[" + _ArrayLength.ToString() + "]";
    }

    public TypeEnum TypeEnum
    {
      get
      {
        if (_TypeEnumIsAssigned == false)
        {
          if (_TypeText == "string")
            _TypeEnum = TypeEnum.String;
          else if (_TypeText == "Size")
            _TypeEnum = TypeEnum.Size;
          else if (_TypeText == "decimal")
            _TypeEnum = TypeEnum.Decimal;
          else if (_TypeText == "int")
            _TypeEnum = TypeEnum.Int;
          else if (_TypeText == "Int32")
            _TypeEnum = TypeEnum.Int32;
          else if (_TypeText == "Int64")
            _TypeEnum = TypeEnum.Int64;
          else if (_TypeText == "bool")
            _TypeEnum = TypeEnum.Bool;
          else if (_TypeText == "void")
            _TypeEnum = TypeEnum.Void;
          else
            _TypeEnum = TypeEnum.none;

          _TypeEnumIsAssigned = true;
        }
        return _TypeEnum;
      }
      set
      {
        _TypeEnum = value;
        _TypeText = null;
        _TypeEnumIsAssigned = true;
      }
    }

    public string TypeText
    {
      get
      {
        if (_TypeText == null)
        {
          _TypeText = CoreEnums.TypeEnumToString(_TypeEnum);
        }
        return _TypeText;
      }
      set
      {
        if (value == "String")
          _TypeText = "string";
        else if (value == "Boolean")
          _TypeText = "bool";
        else
          _TypeText = value;

        _TypeEnumIsAssigned = false;
      }
    }

    public static FrameworkType Void
    {
      get { return new FrameworkType("void"); }
    }

  }
}
