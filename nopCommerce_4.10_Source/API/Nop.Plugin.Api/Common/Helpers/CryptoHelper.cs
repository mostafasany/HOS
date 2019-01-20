using System.IO;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Plugin.Api.Common.Helpers
{
    public static class CryptoHelper
    {
        private const string TokenSigningKeyFileName = "api-token-signing-key.json";

        // Need to ensure that the key would be the same through the application lifetime.
        private static RsaSecurityKey _key;

        public static RsaSecurityKey CreateRsaSecurityKey()
        {
            if (_key == null)
            {
                string pathToKey = CommonHelper.DefaultFileProvider.MapPath($"~/App_Data/{TokenSigningKeyFileName}");

                if (!File.Exists(pathToKey))
                {
                    // generate random parameters
                    RSAParameters randomParameters = GetRandomParameters();

                    var rsaParams = new RSAParametersWithPrivate();
                    rsaParams.SetParameters(randomParameters);
                    string serializedParameters = JsonConvert.SerializeObject(rsaParams);

                    // create file and save the key
                    File.WriteAllText(pathToKey, serializedParameters);
                }

                // load the key
                if (!File.Exists(pathToKey))
                    throw new FileNotFoundException("Check configuration - cannot find auth key file: " + pathToKey);

                var keyParams = JsonConvert.DeserializeObject<RSAParametersWithPrivate>(File.ReadAllText(pathToKey));

                // create signing key by the key above
                _key = new RsaSecurityKey(keyParams.ToRSAParameters());
            }

            return _key;
        }

        public static RSAParameters GetRandomParameters()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    return rsa.ExportParameters(true);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        // https://github.com/mrsheepuk/ASPNETSelfCreatedTokenAuthExample/blob/master/src/TokenAuthExampleWebApplication/RSAKeyUtils.cs
        private class RSAParametersWithPrivate
        {
            public byte[] D { get; set; }
            public byte[] DP { get; set; }
            public byte[] DQ { get; set; }
            public byte[] Exponent { get; set; }
            public byte[] InverseQ { get; set; }
            public byte[] Modulus { get; set; }
            public byte[] P { get; set; }
            public byte[] Q { get; set; }

            public void SetParameters(RSAParameters p)
            {
                D = p.D;
                DP = p.DP;
                DQ = p.DQ;
                Exponent = p.Exponent;
                InverseQ = p.InverseQ;
                Modulus = p.Modulus;
                P = p.P;
                Q = p.Q;
            }

            public RSAParameters ToRSAParameters() => new RSAParameters
            {
                D = D,
                DP = DP,
                DQ = DQ,
                Exponent = Exponent,
                InverseQ = InverseQ,
                Modulus = Modulus,
                P = P,
                Q = Q
            };
        }
    }
}