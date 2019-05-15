using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Cipher
{
  public static class CipherCommon
  {
    public static byte[] EncryptPassword(string Password, string UserName,
      byte[] ServerSeed, byte[] ClientSeed)
    {
      byte[] pwSub = null;
      byte[] paddedPass = Password.ToUpper().PadRight(8, ' ').ToEbcdicBytes();
      byte[] paddedUser = UserName.ToUpper().PadRight(8, ' ').ToEbcdicBytes();

      // pwSeq - byte value 1 padded to left with 0x00 to the lenght of 8 bytes.
      var pwSeq = (new byte[1] { 0x01 }).PadLeft(8, 0x00);

      // initialization vector. set to 8 bytes of hex 00.
      var IV = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

      // exclusive or the padded password with 0x55
      var xorPass = paddedPass.ExclusiveOr(0x55);

      // shift to the left by 1 bit.
      var shiftResult = xorPass.ShiftLeft();

      // DES ECB encode the password.
      var pw_token = paddedUser.EncryptDES(shiftResult, CipherMode.ECB, IV);

      // add PWSEQ to the server seed.
      var rdrSeq = ServerSeed.AddValue(1);

      // padded userName which is exclusive or'd and then doubled to 16 bytes.
      byte[] xorUser = null;
      {
        var pu16 = paddedUser.PadRight(16, 0x40);
        var xor1 = pu16.SubArray(0, 8).ExclusiveOr(rdrSeq);
        var xor2 = pu16.SubArray(8, 8).ExclusiveOr(rdrSeq);
        xorUser = xor1.Concat(xor2);
      }

      // the 40 byte data to pass to CBC encrypt function.
      {
        var dataArray = rdrSeq.Concat(ClientSeed).Concat(xorUser).Concat(pwSeq);

        var res = dataArray.EncryptDES(pw_token, CipherMode.CBC, IV);

        // password substitution value is the final CBC encoded chunk in the encoded
        // result array.
        pwSub = res.SubArray(32, 8);
      }

      return pwSub;
    }

  }
}
