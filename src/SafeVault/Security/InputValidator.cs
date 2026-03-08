using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;

namespace SafeVault.Security
{
    public static class InputValidator
    {
        public static string SanitizeUsername(string username)
        {
            if (username == null) return null;
            var cleaned = Regex.Replace(username, @"[^\w\-.]", "");
            return cleaned.Trim();
        }

        public static string SanitizeEmail(string email)
        {
            if (email == null) return null;
            try
            {
                var addr = new MailAddress(email);
                return addr.Address;
            }
            catch
            {
                return null;
            }
        }

        public static string HtmlEncode(string input)
        {
            if (input == null) return null;
            return WebUtility.HtmlEncode(input);
        }
    }
}
