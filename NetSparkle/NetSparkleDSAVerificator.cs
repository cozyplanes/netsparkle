﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace AppLimit.NetSparkle
{
    public class NetSparkleDSAVerificator
    {
        private DSACryptoServiceProvider _provider;

        public NetSparkleDSAVerificator(String publicKey)
        {
            // 1. try to load this from resource
            Stream data = TryGetResourceStream(publicKey);
            if (data == null )
                data = TryGetFileResource(publicKey, data);

            // 2. check the resource
            if ( data == null )
                throw new Exception("Couldn't find public key for verification");

            // 3. read out the key value
            using (StreamReader reader = new StreamReader(data))
            {
                    String key = reader.ReadToEnd();
                    _provider = new DSACryptoServiceProvider();
                    _provider.FromXmlString(key);
            }            
        }

        public Boolean VerifyDSASignature(String signature, String binaryPath)
        {
            if (_provider == null)
                return false;

            // convert signature
            Byte[] bHash = Convert.FromBase64String(signature);

            // read the data
            byte[] bData = null;
            using (Stream inputStream = File.OpenRead(binaryPath))
            {
                bData = new Byte[inputStream.Length];
                inputStream.Read(bData, 0, bData.Length);
            }
            
            // verify
            return _provider.VerifyData(bData, bHash);            
        }

        private static Stream TryGetFileResource(String publicKey, Stream data)
        {
            if (File.Exists(publicKey))
            {
                data = File.OpenRead(publicKey);
            }
            return data;
        }

        private static Stream TryGetResourceStream(String publicKey)
        {
            Stream data = null;

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                data = asm.GetManifestResourceStream(publicKey);
                if (data != null)
                    break;
            }
            return data;
        }
    }
}