using System.Collections.Generic;
using Utilities;

namespace Localization
{
    public static class LocalizationExtensions
    {
        #region Game Extensions

        #endregion

        public static string Localized(this string self, string targetLoc = null, bool mustExist = true)
        {
            return LocalizationManager.GetText(self, targetLoc, mustExist);
        }

        public static string Localized(this Strings.Keys self, string targetLoc = null)
        {
            return LocalizationManager.GetText(self.ToString(), targetLoc);
        }

        private static List<string> GetKeys(string keyPrefix)
        {
            var list = new List<string>();
            foreach (string key in LocalizationManager.Instance.Data.Keys)
            {
                if (key.StartsWith(keyPrefix))
                    list.Add(key);
            }

            return list;
        }

        private static string GetRandomText(string keyPrefix)
        {
            return GetRandomKey(keyPrefix).Localized();
        }

        private static string GetRandomKey(string keyPrefix)
        {
            return GetKeys(keyPrefix).RandomElement();
        }
    }
}
