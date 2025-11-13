using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services
{
    public class VNPayHelper
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            byte[] hashValue = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            StringBuilder hex = new StringBuilder(hashValue.Length * 2);
            foreach (byte b in hashValue)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
        public static string BuildQueryUrl(string baseUrl, SortedDictionary<string, string> parameters, string hashSecret)
        {
            var signData = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));

            string secureHash = HmacSHA512(hashSecret, signData);

            var query = string.Join("&", parameters.Select(p => $"{p.Key}={HttpUtility.UrlEncode(p.Value)}"));

            return $"{baseUrl}?{query}&vnp_SecureHash={secureHash}";
        }

        public static bool ValidateSignature(IReadOnlyDictionary<string, string> parameters, string secret)
        {
            string receivedHash = parameters["vnp_SecureHash"];

            var sorted = new SortedDictionary<string, string>();
            foreach (var pair in parameters)
            {
                if (pair.Key.StartsWith("vnp_") && pair.Key != "vnp_SecureHash")
                    sorted.Add(pair.Key, pair.Value);
            }

            string rawData = string.Join("&", sorted.Select(x => $"{x.Key}={x.Value}"));
            string computedHash = HmacSHA512(secret, rawData);

            return string.Equals(computedHash, receivedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
