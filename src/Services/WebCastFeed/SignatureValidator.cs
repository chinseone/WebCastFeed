using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebCastFeed.Models.Requests;

namespace WebCastFeed
{
    public static class SignatureValidator
    {
        public static bool ValidateSignature(
            string cmd,
            string timestamp,
            string version,
            string push_id,
            string nonce_str,
            string signature,
            List<DouyinMessage> request)
        {
            var sigValidationEnabled = bool.Parse(Environment.GetEnvironmentVariable("ValidateSignature") ?? "false");
            Console.WriteLine($"Validate signature: {sigValidationEnabled}");
            if (sigValidationEnabled)
            {
                Console.WriteLine($"cmd={cmd}");
                Console.WriteLine($"timestamp={timestamp}");
                Console.WriteLine($"version={version}");
                Console.WriteLine($"push_id={push_id}");
                Console.WriteLine($"nonce_str={nonce_str}");
                Console.WriteLine($"signature={signature}");
                Console.WriteLine($"body={JsonConvert.SerializeObject(request)}");
            }
            return true;
        }
    }
}
