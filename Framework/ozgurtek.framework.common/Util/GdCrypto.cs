using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ozgurtek.framework.common.Util
{
    public static class GdCrypto
    {
        public static string Encrypt(string value, string key, bool useHashing)
        {
            byte[] valueArray = Encoding.UTF8.GetBytes(value);

            byte[] keyArray;
            if (useHashing)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                keyArray = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                md5.Clear();
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider();
            tripleDes.Key = keyArray;
            tripleDes.Mode = CipherMode.ECB;
            tripleDes.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = tripleDes.CreateEncryptor();
            byte[] resultArray = transform.TransformFinalBlock(valueArray, 0, valueArray.Length);
            tripleDes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherValue, string key, bool useHashing)
        {
            byte[] cipherValueArray = Convert.FromBase64String(cipherValue);

            byte[] keyArray;
            if (useHashing)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                keyArray = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                md5.Clear();
            }
            else
            {
                keyArray = Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider();
            tripleDes.Key = keyArray;
            tripleDes.Mode = CipherMode.ECB;
            tripleDes.Padding = PaddingMode.PKCS7;

            ICryptoTransform transform = tripleDes.CreateDecryptor();
            byte[] resultArray = transform.TransformFinalBlock(cipherValueArray, 0, cipherValueArray.Length);
            tripleDes.Clear();

            return Encoding.UTF8.GetString(resultArray);
        }

        public static string EncryptSimple(string inputStr)
        {
            const int sum = 10;

            int len = inputStr.Length;
            char[] encryptedChars = new char[len];
            for (int i = 0; i < len; i++)
            {
                encryptedChars[i] = (char)(inputStr[i] + sum);
            }

            char[] newNameStr = new char[len];
            for (int i = 0; i < len; i++)
            { // reverse the name
                newNameStr[i] = encryptedChars[len - 1 - i];
            }

            for (int i = 0; i < len; i++)
            { // reverse the name
                encryptedChars[i] = newNameStr[i];
            }

            return String.Intern(new String(encryptedChars));
        }

        public static string DecryptSimple(string inputStr)
        {
            const int sum = 10;

            int len = inputStr.Length;
            char[] decryptedChars = new char[len];
            for (int i = 0; i < len; i++)
            {
                decryptedChars[i] = (char)((int)inputStr[i] - sum);
            }

            char[] newNameStr = new char[len];
            for (int i = 0; i < len; i++)
            { // reverse the name
                newNameStr[i] = decryptedChars[len - 1 - i];
            }

            for (int i = 0; i < len; i++)
            { // reverse the name
                decryptedChars[i] = newNameStr[i];
            }

            return String.Intern(new String(decryptedChars));
        }

        public static string EncyrptMd5(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            string hashValue = BitConverter.ToString(bytes);
            string result = hashValue.Replace("-", "");
            md5.Dispose();

            return result;
        }

        public static string EncryptAes(string cipherText)
        {
            byte[] keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            byte[] iv = Encoding.UTF8.GetBytes("8080808080808080");

            byte[] encrypted;
            // Create a RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = keybytes;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.  
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.  
                            swEncrypt.Write(cipherText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.  
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptAes(string cipherText)
        {
            byte[] keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            byte[] iv = Encoding.UTF8.GetBytes("8080808080808080");

            byte[] encrypted = Convert.FromBase64String(cipherText);

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = keybytes;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(encrypted))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return string.Format(plaintext);
        }
    }
}
