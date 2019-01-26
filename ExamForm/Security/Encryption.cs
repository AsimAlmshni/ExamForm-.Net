using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ExamForm.Security
{
    public class Encryption
    {
        //public static string Encrypt(string clearText)
        //{
        //    string EncryptionKey = "MAKV2SPBNI99212";
        //    byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        //    using (Aes encryptor = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //        encryptor.Key = pdb.GetBytes(32);
        //        encryptor.IV = pdb.GetBytes(16);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        //            {
        //                cs.Write(clearBytes, 0, clearBytes.Length);
        //                cs.Close();
        //            }
        //            clearText = Convert.ToBase64String(ms.ToArray());
        //        }
        //    }
        //    return clearText;
        //}

        public static string Encrypt(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            //encrypt data
            var data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);

            //return as base64 string
            return Convert.ToBase64String(encrypted);
        }

        //public static string Encrypt(string openText)
        //{
        //    RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();
        //    ICryptoTransform encryptor = rc2CSP.CreateEncryptor(Convert.FromBase64String(rc2CSP.Key), Convert.FromBase64String(rc2CSP.IV));
        //    using (MemoryStream msEncrypt = new MemoryStream())
        //    {
        //        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //        {
        //            byte[] toEncrypt = Encoding.Unicode.GetBytes(openText);

        //            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
        //            csEncrypt.FlushFinalBlock();

        //            byte[] encrypted = msEncrypt.ToArray();

        //            return Convert.ToBase64String(encrypted);
        //        }
        //    }
        //}

    }
}