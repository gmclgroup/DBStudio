using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Security.Cryptography;
using System.Collections;

namespace WSESimpleTCPDLL
{
    ///
    /// One static method to get an RSACryptoServiceProvider from a *.snk file.
    /// NOTE:  These methods assume 1024 bit keys, the same as exported from sn.exe.
    ///
    public sealed class SnkUtil
    {
        #region Fields
        private const int magic_priv_idx = 0x08;
        private const int magic_pub_idx = 0x14;
        private const int magic_size = 4;
        #endregion

        #region Constructors
        private SnkUtil()
        {
        }
        #endregion

        #region Public Methods
        ///
        /// Returns RSA object from *.snk key file.
        ///
        ///
        ///Path to snk file.
        /// RSACryptoServiceProvider
        public static RSACryptoServiceProvider GetRSAFromSnkFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            byte[] snkBytes = GetFileBytes(path);
            if (snkBytes == null)
                throw new Exception("Invalid SNK file.");

            RSACryptoServiceProvider rsa = GetRSAFromSnkBytes(snkBytes);
            return rsa;
        }

        public static RSACryptoServiceProvider GetPublicKeyFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            byte[] pubkey = assembly.GetName().GetPublicKey();
            if (pubkey.Length == 0)
                throw new Exception("No public key in assembly.");

            RSAParameters p = SnkUtil.GetRSAParameters(pubkey);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(p);
            return rsa;
        }

        ///
        /// Returns RSAParameters from byte[].
        /// Example to get rsa public key from assembly:
        /// byte[] pubkey = System.Reflection.Assembly.GetExecutingAssembly().GetName().GetPublicKey();
        /// RSAParameters p = SnkUtil.GetRSAParameters(pubkey);
        ///
        ///

        ///
        public static RSAParameters GetRSAParameters(byte[] keyBytes)
  {
   RSAParameters ret = new RSAParameters();

   if ((keyBytes == null) || (keyBytes.Length < 1))
    throw new ArgumentNullException("keyBytes");     
   
   bool pubonly = SnkBufIsPubLength(keyBytes);

   if ((pubonly) && (!CheckRSA1(keyBytes)))
    return ret;
      
   if ((!pubonly) && (!CheckRSA2(keyBytes)))
    return ret;
     
   int magic_idx = pubonly==true? magic_pub_idx : magic_priv_idx;
   
   // Bitlen is stored here, but note this
   // class is only set up for 1024 bit length keys
   int bitlen_idx = magic_idx + magic_size;
   int bitlen_size = 4;  // DWORD

   // Exponent
   // In read file, will usually be { 1, 0, 1, 0 } or 65537
   int exp_idx = bitlen_idx + bitlen_size;
   int exp_size = 4;


   //BYTE modulus[rsapubkey.bitlen/8]; == MOD; Size 128
   int mod_idx = exp_idx + exp_size;
   int mod_size = 128;  

   //BYTE prime1[rsapubkey.bitlen/16]; == P; Size 64
   int p_idx = mod_idx + mod_size;
   int p_size = 64;

   //BYTE prime2[rsapubkey.bitlen/16]; == Q; Size 64   
   int q_idx = p_idx + p_size;
   int q_size = 64;

   //BYTE exponent1[rsapubkey.bitlen/16]; == DP; Size 64
   int dp_idx = q_idx + q_size;
   int dp_size = 64;

   //BYTE exponent2[rsapubkey.bitlen/16]; == DQ; Size 64   
   int dq_idx = dp_idx + dp_size;
   int dq_size = 64;

   //BYTE coefficient[rsapubkey.bitlen/16]; == InverseQ; Size 64
   int invq_idx = dq_idx + dq_size;
   int invq_size = 64;

   //BYTE privateExponent[rsapubkey.bitlen/8]; == D; Size 128
   int d_idx = invq_idx + invq_size;
   int d_size = 128;
       

   // Figure public params
   // Must reverse order (little vs. big endian issue)
   ret.Exponent = BlockCopy(keyBytes, exp_idx, exp_size);
   Array.Reverse(ret.Exponent);
   ret.Modulus = BlockCopy(keyBytes, mod_idx, mod_size);
   Array.Reverse(ret.Modulus);
     
   if (pubonly) return ret;

   // Figure private params    
   // Must reverse order (little vs. big endian issue)
   ret.P = BlockCopy(keyBytes, p_idx, p_size);
   Array.Reverse(ret.P);

   ret.Q = BlockCopy(keyBytes, q_idx, q_size);
   Array.Reverse(ret.Q);

   ret.DP = BlockCopy(keyBytes, dp_idx, dp_size);
   Array.Reverse(ret.DP);

   ret.DQ = BlockCopy(keyBytes, dq_idx, dq_size);
   Array.Reverse(ret.DQ);

   ret.InverseQ = BlockCopy(keyBytes, invq_idx, invq_size);
   Array.Reverse(ret.InverseQ);

   ret.D = BlockCopy(keyBytes, d_idx, d_size);
   Array.Reverse(ret.D);
   
   return ret;
  }
        #endregion

        #region Private Methods
        private static byte[] GetFileBytes(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] bytes = br.ReadBytes((int)fs.Length);
                return bytes;
            }
        }

        private static RSACryptoServiceProvider GetRSAFromSnkBytes(byte[] snkBytes)
        {
            if (snkBytes == null)
                throw new ArgumentNullException("snkBytes");

            RSAParameters param = GetRSAParameters(snkBytes);

            // Must set KeyNumber to AT_SIGNATURE for strong
            // name keypair to be correctly imported.     
            CspParameters cp = new CspParameters();
            cp.KeyNumber = 2; // AT_SIGNATURE

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024, cp);
            rsa.ImportParameters(param);
            return rsa;
        }

        private static byte[] BlockCopy(byte[] source, int idx, int size)
        {
            if ((source == null) || (source.Length < (idx + size)))
                return null;

            byte[] ret = new byte[size];
            Buffer.BlockCopy(source, idx, ret, 0, size);
            return ret;
        }

        ///
        /// Returns true if buffer length is public key size.
        ///
        ///

        ///
        private static bool SnkBufIsPubLength(byte[] keypair)
        {
            if (keypair == null)
                return false;
            return (keypair.Length == 160);
        }

        ///
        /// Check that RSA1 is in header (public key only).
        ///
        ///

        ///
        private static bool CheckRSA1(byte[] pubkey)
        {
            // Check that RSA1 is in header.
            //                             R     S     A     1
            byte[] check = new byte[] { 0x52, 0x53, 0x41, 0x31 };
            return CheckMagic(pubkey, check, magic_pub_idx);
        }

        ///
        /// Check that RSA2 is in header (public and private key).
        ///
        ///

        ///
        private static bool CheckRSA2(byte[] pubkey)
        {
            // Check that RSA2 is in header.
            //                             R     S     A     2
            byte[] check = new byte[] { 0x52, 0x53, 0x41, 0x32 };
            return CheckMagic(pubkey, check, magic_priv_idx);
        }

        private static bool CheckMagic(byte[] keypair, byte[] check, int idx)
        {
            byte[] magic = BlockCopy(keypair, idx, magic_size);
            if (magic == null)
                return false;

            for (int i = 0; i < magic_size; i++)
            {
                if (check[i] != magic[i])
                    return false;
            }

            return true;
        }
        #endregion
    }
}
