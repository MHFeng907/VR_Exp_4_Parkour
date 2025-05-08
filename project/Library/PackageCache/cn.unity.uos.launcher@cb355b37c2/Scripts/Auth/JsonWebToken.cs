using System;
using System.Text;
using Newtonsoft.Json;
using Unity.UOS.Common.UOSLauncher.Scripts.Auth;

namespace Unity.UOS.Auth
{
    internal struct JsonWebToken
    {
        private static readonly char[] JwtSeparator = { '.' };
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public DateTime Expiration { get; }

        [JsonConstructor]
        public JsonWebToken(long exp)
        {
            Expiration = UnixEpoch.AddSeconds(exp);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static JsonWebToken Decode(string token)
        {
            var parts = token.Split(JwtSeparator);
            if (parts.Length != 3)
            {
                throw new AuthException(ErrorCode.InvalidJwt, "invalid jwt");
            }

            var payloadString = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payloadString));
            var payload = JsonConvert.DeserializeObject<Payload>(payloadJson);
            return new JsonWebToken(payload.exp);
        }

        static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding

            var mod4 = input.Length % 4;
            if (mod4 > 0)
            {
                output += new string('=', 4 - mod4);
            }

            return Convert.FromBase64String(output);
        }
    }

    [Serializable]
    internal struct Payload
    {
        public int exp;
    }
}