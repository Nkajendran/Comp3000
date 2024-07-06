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
            // Initialization-This is an input variable used to make sure the encryption taking place is more secure
            //PlaintextBytes -This is the input text which gets converted into bytes
            //keyBytes- These are bytes used in the key generation
            //SymmetricKey.Blocksize- This is the size of the key.
            //SymmetricKey.Mode- This is the mode to use.
            //SymmetricKey.Padding- Padding added to the key.
            var Salt = Generate128BitsOfRandomEntropy();
            var IV = Generate128BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, Salt, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, IV))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                var cipherTextBytes = Salt;
                                cipherTextBytes = cipherTextBytes.Concat(IV).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }
        public static string Decrypt(string cipherText, string passPhrase)
        {
            //This section is for the decryption to display the information for the user.
            //All the variables such as string bytes and TextBytes generates a key based on the password
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
           
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
           
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
           
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
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