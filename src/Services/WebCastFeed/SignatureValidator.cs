using System;
using System.Collections.Generic;
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

            }
            return true;
        }
    }
}
