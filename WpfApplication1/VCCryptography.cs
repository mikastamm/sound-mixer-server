using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SoundMixerServer
{
    public class VCCryptography
    {
        public static int SERVER_KEY_SIZE = 2048;
        public static int CLIENT_KEY_SIZE = 2048;
        
        private static string[] getSegments(string msg)
        {
            int segmentSize = getMaximumDataBytes(CLIENT_KEY_SIZE, 160);
            int segmentCount = (int)Math.Ceiling(((double)msg.Length) / segmentSize);
            string[] segments = new string[segmentCount];

            for (int i = 0; i < segmentCount; i++)
            {
                if (msg.Length >= segmentSize)
                {
                    segments[i] = msg.Substring(0, segmentSize);
                    msg = msg.Remove(0, segmentSize);
                }
                else
                {
                    segments[i] = msg;
                }
            }

            return segments;
        }

        [Obsolete]
        public static string getEncryptedMessage(string msg, RSAKeyValue key)
        {
            string[] segments = getSegments(msg);
            string[] encryptedSegments = new string[segments.Length];

            for (int i = 0; i < segments.Length; i++)
            {
                encryptedSegments[i] = Convert.ToBase64String(RSAEncrypt(Encoding.UTF8.GetBytes(segments[i]), key));
            }

            string JSON = JSONManager.serialize(encryptedSegments);

            return JSON;
        }

        private static int getMaximumDataBytes(int keySizeBits, int hashOutputSizeBits)
        {
            return keySizeBits / 8 - 2 * hashOutputSizeBits / 8 - 2;
        }

        public static byte[] RSAEncrypt(byte[] toEncrypt, RSAKeyValue key)
        {
            byte[] encrypted;

            RSAParameters parameters = new RSAParameters();
            parameters.Modulus = Convert.FromBase64String(key.Modulus);
            parameters.Exponent = Convert.FromBase64String(key.Exponent);

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    RSA.ImportParameters(parameters);
                    encrypted = RSA.Encrypt(toEncrypt, true);
                }
                finally
                {
                    RSA.PersistKeyInCsp = false;
                }
            }

            return encrypted;
        }

        public static byte[] RSADecrypt(byte[] byteDecrypt)
        {
            try
            {
                byte[] decryptedData;

                CspParameters param = new CspParameters();
                param.KeyContainerName = "VC_RSA_KEY";
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048, param))
                {
                    //Decrypt the passed byte array and specify OAEP padding.
                    decryptedData = RSA.Decrypt(byteDecrypt, true);
                }
                return decryptedData;
              }
        //Catch and display a CryptographicException
        //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }  
        }

        public static string getPublicKey()
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = "VC_RSA_KEY";
            //Create a new instance of RSACryptoServiceProvider.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048, param))
            {
                string xmlKey = RSA.ToXmlString(false);
                RSAKeyValue key = XMLManager.deserialize<RSAKeyValue>(xmlKey);
                Console.WriteLine("MOD:" + Convert.FromBase64String(key.Modulus).Length *8 );
                string jsonKey = JSONManager.serialize(key);

                return jsonKey;
            }
        }
    }
    public class RSAKeyValue
    {
        public string Modulus { get; set; }
        public string Exponent { get; set; }
    }
}
