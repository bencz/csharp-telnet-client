using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace AutoCoder.Ext.System.Reflection.Emit
{
  public static class TypeBuilderExt
  {
    public static PropertyBuilder AddProperty<T>(this TypeBuilder tb, string propName)
    {
      var fldName = "_" + propName;

      FieldBuilder fb =
        tb.DefineField(fldName, typeof(T), FieldAttributes.Private);

      PropertyBuilder pb = tb.DefineProperty(
        propName, PropertyAttributes.HasDefault, typeof(T), null);

      // The property set and property get methods require a special
      // set of attributes.
      MethodAttributes getSetAttr =
          MethodAttributes.Public | MethodAttributes.SpecialName |
              MethodAttributes.HideBySig;

      // Define the "get" accessor method for CustomerName.
      MethodBuilder getMb =
          tb.DefineMethod("get_" + propName, getSetAttr, typeof(T), Type.EmptyTypes);

      {
        var ilg = getMb.GetILGenerator();

        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldfld, fb);
        ilg.Emit(OpCodes.Ret);
      }

      // Define the "set" accessor method for CustomerName.
      MethodBuilder setMb = tb.DefineMethod(
        "set_" + propName, getSetAttr, null, new Type[] { typeof(T) });

      {
        var ilg = setMb.GetILGenerator();

        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldarg_1);
        ilg.Emit(OpCodes.Stfld, fb);
        ilg.Emit(OpCodes.Ret);
      }

      // Last, we must map the two methods created above to our PropertyBuilder to 
      // their corresponding behaviors, "get" and "set" respectively. 
      pb.SetGetMethod(getMb);
      pb.SetSetMethod(setMb);

      return pb;
    }

    public static PropertyBuilder AddStringProperty(this TypeBuilder tb, string propName)
    {
      var fldName = "_" + propName;

      FieldBuilder fb =
        tb.DefineField(fldName, typeof(string), FieldAttributes.Private);

      PropertyBuilder pb = tb.DefineProperty(
        propName, PropertyAttributes.HasDefault, typeof(string), null);

      // The property set and property get methods require a special
      // set of attributes.
      MethodAttributes getSetAttr =
          MethodAttributes.Public | MethodAttributes.SpecialName |
              MethodAttributes.HideBySig;

      // Define the "get" accessor method for CustomerName.
      MethodBuilder getMb =
          tb.DefineMethod("get_" + propName, getSetAttr, typeof(string), Type.EmptyTypes);

      {
        var ilg = getMb.GetILGenerator();

        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldfld, fb);
        ilg.Emit(OpCodes.Ret);
      }

      // Define the "set" accessor method for CustomerName.
      MethodBuilder setMb = tb.DefineMethod(
        "set_" + propName, getSetAttr, null, new Type[] { typeof(string) });

      {
        var ilg = setMb.GetILGenerator();

        ilg.Emit(OpCodes.Ldarg_0);
        ilg.Emit(OpCodes.Ldarg_1);
        ilg.Emit(OpCodes.Stfld, fb);
        ilg.Emit(OpCodes.Ret);
      }

      // Last, we must map the two methods created above to our PropertyBuilder to 
      // their corresponding behaviors, "get" and "set" respectively. 
      pb.SetGetMethod(getMb);
      pb.SetSetMethod(setMb);

      return pb;
    }
  }
}
