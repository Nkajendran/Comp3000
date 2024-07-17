using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace DEBA.Controllers
{
    public static class Encryption
    {
        // The Keysize is the size in bytes of the key generated
        private const int Keysize = 128;

        // The derivativeIterations is the number of iterations the password bytes generated
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt- This is a random data used to hash data
            // Initialization Vector-This is an input variable used to make sure the encryption taking place is more secure
            //PlaintextBytes -This is the input text which gets converted into bytes
            //keyBytes- These are bytes used in the key generation
            //SymmetricKey.Blocksize- This is the size of the key.
            //SymmetricKey.Mode- This is the mode to use.
            //SymmetricKey.Padding- Padding added to the key.
            var Salt = Generate128BitsOfRandomEntropy();
            var IV = Generate128BitsOfRandomEntropy();
            var PLAINTEXTBYTES = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, Salt, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var SYMMETRICKEY = new RijndaelManaged())
                {
                     SYMMETRICKEY.BlockSize = 128;
                    SYMMETRICKEY.Mode = CipherMode.CBC;
                    SYMMETRICKEY.Padding = PaddingMode.PKCS7;
                    using (var encryptor = SYMMETRICKEY.CreateEncryptor(keyBytes, IV))
                    {
                        using (var MEMORY_STREAM = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(MEMORY_STREAM, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(PLAINTEXTBYTES, 0, PLAINTEXTBYTES.Length);
                                cryptoStream.FlushFinalBlock();

                                var CIPHERTEXT = Salt;
                                CIPHERTEXT = CIPHERTEXT.Concat(IV).ToArray();
                                CIPHERTEXT = CIPHERTEXT.Concat(MEMORY_STREAM.ToArray()).ToArray();
                                MEMORY_STREAM.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(CIPHERTEXT);
                            }
                        }
                    }
                }
            }
        }
        public static string Decrypt(string CIPHERTEXT, string passPhrase)
        {
            //This section is for the decryption to display the information for the user.
            //All the variables such as string bytes and TextBytes generates a key based on the password
            var CIPHERTEXTBytesWithSaltAndIv = Convert.FromBase64String(CIPHERTEXT);
           
            var saltStringBytes = CIPHERTEXTBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
           
            var ivStringBytes = CIPHERTEXTBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
           
            var cipherTextBytes = CIPHERTEXTBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(CIPHERTEXTBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var SYMMETRICKEY_2 = new RijndaelManaged())
                {
                    SYMMETRICKEY_2.BlockSize = 128;
                    SYMMETRICKEY_2.Mode = CipherMode.CBC;
                    SYMMETRICKEY_2.Padding = PaddingMode.PKCS7;
                    using (var decryptor = SYMMETRICKEY_2.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var MEMORYSTREAM_2 = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(MEMORYSTREAM_2, decryptor, CryptoStreamMode.Read))
                            using (var streamReader = new StreamReader(MEMORYSTREAM_2, Encoding.UTF8))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        // The final part generates random data.
        // This function generates random data. This task is performed by the calculations.
        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; 
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
 
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }

}