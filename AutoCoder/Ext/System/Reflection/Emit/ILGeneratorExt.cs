using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace AutoCoder.Ext.System.Reflection.Emit
{
  public static class ILGeneratorExt
  {
    public static void LoadRefArrayElem(
      this ILGenerator ilg, OpCode loadOpcode, int index)
    {
      ilg.Emit(loadOpcode);

      ilg.LoadIndex(index);

      // load the ref array element.
      ilg.Emit(OpCodes.Ldelem_Ref);
    }

    public static void LoadStoreRefArrayElem(
      this ILGenerator ilg, OpCode loadOpcode, int index, OpCode storeOpcode)
    {
      ilg.Emit(loadOpcode);

      ilg.LoadIndex(index);

      // load the ref array element.
      ilg.Emit(OpCodes.Ldelem_Ref);

      // store the loaded element.
      ilg.Emit(storeOpcode);
    }

    public static void LoadIndex(this ILGenerator ilg, int index )
    {
      // setup opcode to load the array element index.
      OpCode? loadIndex = null;
      if (index == 0)
        loadIndex = OpCodes.Ldc_I4_0;
      else if (index == 1)
        loadIndex = OpCodes.Ldc_I4_1;
      else if (index == 2)
        loadIndex = OpCodes.Ldc_I4_2;
      else if (index == 3)
        loadIndex = OpCodes.Ldc_I4_3;
      else if (index == 4)
        loadIndex = OpCodes.Ldc_I4_4;
      else if (index == 5)
        loadIndex = OpCodes.Ldc_I4_5;
      else if (index == 6)
        loadIndex = OpCodes.Ldc_I4_6;
      else if (index == 7)
        loadIndex = OpCodes.Ldc_I4_7;

      // load the array element index.
      ilg.Emit(loadIndex.Value);
    }
  }
}
