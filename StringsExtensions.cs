using System;
using System.Text.RegularExpressions;

namespace PixelIT.web
{
    /// <summary>
    /// Alles rund um Strings
    /// </summary>
    public static class StringsExtensions
    {
        /// <summary>
        /// Im String die Umlaute ersetzten.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UmlauteReplace(this string input)
        {
            var result = input.Replace("ö", "oe");
            result = result.Replace("Ö", "Oe");
            result = result.Replace("ü", "ue");
            result = result.Replace("Ü", "Ue");
            result = result.Replace("ä", "ae");
            result = result.Replace("Ä", "Ae");
            result = result.Replace("ß", "ss");
            return result;
        }

        /// <summary>
        /// Aus einen String die Sonderzeichen raus Filtern.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SonderzeichenRemove(this string input)
        {
            Regex pattern = new("[^a-zA-Z0-9]");
            return pattern.Replace(input, "");
        }

        /// <summary>
        /// Username aus einen Windowslogin filtern.
        /// </summary>
        /// <param name="login">Windows Login</param>
        /// <returns>Username</returns>
        public static string GetUsernameFromUserLogin(this string login)
        {
            int stop = login.IndexOf("\\");
            return (stop > -1) ? login.Substring(stop + 1, login.Length - stop - 1) : string.Empty;
        }

        /// <summary>
        /// Domain aus einen Windowslogin filtern.
        /// </summary>
        /// <param name="login">Windows Login</param>
        /// <returns>Domain</returns>
        public static string GetDomainFromUserLogin(this string login)
        {
            int stop = login.IndexOf("\\");
            return (stop > -1) ? login.Substring(0, stop) : string.Empty;
        }

        /// <summary>
        /// Prüfen ob ein String eine gültiger Alpha-numerischer String ist.
        /// </summary>
        /// <param name="strAlphanum"></param>
        /// <returns></returns>
        public static bool IsAlphaNumericString(this string strAlphanum)
        {
            Regex pattern = new(@"^[A-Za-z0-9]+$");
            return pattern.IsMatch(strAlphanum.Trim());
        }

        /// <summary>
        /// Prüft einen String ob er eine Nummer ist.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>bool</returns>
        public static bool IsNumber(this string input)
        {
            return long.TryParse(input, out _);
        }

        /// <summary>
        /// Prüft ob der String eine IP ist.
        /// </summary>
        /// <param name="IP"></param>
        /// <returns>bool</returns>
        public static bool IsIP(this string IP)
        {
            return Regex.IsMatch(IP, @"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$\b");
        }

        /// <summary>
        /// Extrahiert eine Nummer aus einen String
        /// </summary>
        /// <param name="input"></param>
        /// <returns>bool</returns>
        public static int ExtractNumber(this string input)
        {
            string output = Regex.Replace(input, @"\D", string.Empty);
            return int.Parse(output);
        }

        /// <summary>
        /// Prüft ob ein String eine Nummer enthält
        /// </summary>
        /// <param name="input"></param>
        /// <returns>bool</returns>
        public static bool ContainsNumber(this string input)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string output = Regex.Replace(input, @"\D", string.Empty);
            return !String.IsNullOrWhiteSpace(output);
        }

        /// <summary>
        /// Löscht unötige Leerzeichen!
        /// </summary>
        /// <param name="input">The text.</param>
        /// <returns></returns>
        public static string RemoveMultiSpaceCharacters(this string input)
        {
            return Regex.Replace(input, "[ ]+", " ");
        }

        /// <summary>
        /// Prüft ob ein Regex gültig ist.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsValidRegex(this string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Konvertiert einen String zu einen Base64String.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            if (String.IsNullOrEmpty(plainText))
            {
                return String.Empty;
            }
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Kovertiert einen Base64String zu einen String.
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            if (String.IsNullOrEmpty(base64EncodedData))
            {
                return String.Empty;
            }
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
