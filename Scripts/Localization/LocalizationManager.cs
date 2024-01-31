using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using Utilities;

namespace Localization {
    [System.Serializable]
    public class LocalizationText
    {
        public string textId;
        public Dictionary<string, string> textLocals;
    }

    public class LocaleDetail
    {
        public string Code
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool IsRightToLeft
        {
            get;
            private set;
        }
    
        public FontStyle? FontStyle
        {
            get;
            private set;
        }

        public float FontSizeMultiplier
        {
            get;
            private set;
        }

        public SystemLanguage[] SystemMap
        {
            get;
            private set;
        }

        public string[] FbMap
        {
            get;
            private set;
        }

        public string FlagIconName
        {
            get;
            private set;
        }
    }

    public class LocalizationManager
    {
        public const string DefaultLocale = "en";

        private const string PrefsVersionKey = "LocalizationVersion";
        private const string PrefsDataKey = "LocalizationData";

        public static readonly CultureInfo NumberFormatCulture = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");

        public readonly Dictionary<string, LocalizationText> Data = new Dictionary<string, LocalizationText>(600);
        public readonly Dictionary<string, LocaleDetail> Locales = new Dictionary<string, LocaleDetail>(12);

        public static event Action<string, string> LocalizationUpdated = delegate { };

        public int CurrentVersion
        {
            get;
            private set;
        }

        public string CurrentLocale
        {
            get
            {
                if (Detail == null)
                    return DefaultLocale;

                return Detail.Code;
            }
        }

        public LocaleDetail Detail
        {
            get;
            private set;
        }

        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get { return _instance ?? (_instance = new LocalizationManager()); }
        }
    
        private LocalizationManager() 
        {
            ReloadLocalization();
        }

        public void ReloadLocalization(bool forceFromResources = false)
        {
            //DebugSafe.LogFormat(DebugType.UI, "Loading localization, {0}", Time.realtimeSinceStartup);

            var versionAsset = (TextAsset)Resources.Load("StringsVersion");
            int resourcesVersion = int.Parse(versionAsset.text);
            CurrentVersion = resourcesVersion;

            byte[] rawData = null;
            if (!forceFromResources)
            {
                int prefsVersion = PlayerPrefs.GetInt(PrefsVersionKey, 0);
                if (prefsVersion > CurrentVersion)
                {
                    string base64Data = PlayerPrefs.GetString(PrefsDataKey, "");
                    if (!String.IsNullOrEmpty(base64Data))
                    {
                        try
                        {
                            // NOTE: ������ ����������� ��� ����� � Base64, ����� ��������� �� ����� ��������� ��� ���������� / ������ �� PlayerPrefs.
                            rawData = Convert.FromBase64String(base64Data);
                            CurrentVersion = prefsVersion;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("Failed to parse store localization data: " + ex.Message);
                        }
                    }
                }
            
                if (rawData.IsNullOrEmpty())
                {
                    PlayerPrefs.DeleteKey(PrefsVersionKey);
                    PlayerPrefs.DeleteKey(PrefsDataKey);
                }
            }

            if (rawData.IsNullOrEmpty())
            {
                var textAsset = (TextAsset) Resources.Load("Strings");
                rawData = textAsset.bytes;

                // ������������ � ������ �� ��������, ���� ���-�� ����� �� ���.
                CurrentVersion = resourcesVersion;
            }

            string stringData = Encoding.UTF8.GetString(rawData);
            ReloadLocalization(stringData);

            //DebugSafe.LogFormat(DebugType.UI, "Completed Loading localization, {0}", Time.realtimeSinceStartup);
        }

        public void RefreshCurrentLocale(bool canRaiseEvent = true)
        {        
            string prevLocale = CurrentLocale;

            string newLocale = null;// LocalConfig.ForcedLocale;

            // ���������, ��������� �� ��������� �����������
            if (!newLocale.IsNullOrEmpty() && !Locales.ContainsKey(newLocale))
            {
                Debug.LogWarningFormat("Forced localization '{0}' does not exist. Detecting automatically.", newLocale);
                newLocale = null;
            }

            // ���� ��� ����������� �����������, ���������� � �������������
            if (newLocale.IsNullOrEmpty())
            {
                foreach (var pair in Locales)
                {
                    if (pair.Value.SystemMap != null && pair.Value.SystemMap.Contains(Application.systemLanguage))
                    {
                        newLocale = pair.Key;
                        break;
                    }
                }

                newLocale = newLocale ?? DefaultLocale;
            }

            if (!Locales.TryGetValue(newLocale, out var detail))
            {
                Debug.LogWarningFormat("Localization '{0}' does not exist. Fallback to default.", newLocale);
                newLocale = DefaultLocale;
                detail = Locales[DefaultLocale];
            }

            Detail = detail;        

            if (canRaiseEvent && newLocale != prevLocale)
                LocalizationUpdated(prevLocale, CurrentLocale);
        }

        public void ReloadLocalization(string rawData)
        {
            string[] lines = rawData.Split('\n');
            var data = new string[lines.Length][];
            for (var i = 0; i < lines.Length; i++)
                data[i] = lines[i].Split('\t');

            Locales.Clear();
            var localeIndexes = new Dictionary<string, int>(11);
            for (int i = 2; i < data[1].Length; i++)
            {
                if (data[1][i].Trim().Length > 0)
                {
                    var detail = JsonFx.Json.JsonReader.Deserialize<LocaleDetail>(data[1][i]);
                    localeIndexes.Add(detail.Code, i);
                    Locales.Add(detail.Code, detail);
                }
                else
                {
                    break;
                }
            }

            Data.Clear();
            for(var i = 2; i < data.Length; ++i)
            {
                var lt = new LocalizationText
                {
                    textId = data[i][0], 
                    textLocals = new Dictionary<string, string>()
                };
            
                foreach (var kvp in localeIndexes)
                {                
                    lt.textLocals.Add(kvp.Key, data[i][kvp.Value].Replace("\\n", "\n").Replace("\\t", "\t").TrimEnd('\r'));
                }

#if UNITY_EDITOR
                if (Data.ContainsKey(lt.textId))
                    Debug.LogError("Duplicate key: " + lt.textId);
#endif
                Data.Add(lt.textId, lt);
            }
        
            string prevLocale = CurrentLocale;

            // ��������� ��� ������� ������ � ��������� �������, �.�. ����� ���������, ��� �������� ����������� ���� � ������.
            RefreshCurrentLocale(canRaiseEvent: false);

            LocalizationUpdated(prevLocale, CurrentLocale);
        }

#if !LIVE_OPS_IMPORTER
        public void UpdateLocalizationVersion(string rawData, string version)
        {
            try
            {
                ReloadLocalization(rawData);

                // ��������� ����������� � PlayerPrefs, ����� �� ������ ��������.
                CurrentVersion = int.Parse(version);

                // NOTE: ������ ����������� ��� ����� � Base64, ����� ��������� �� ����� ����������� ��� ���������� / ������ �� PlayerPrefs.
                string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawData));

                if (base64Data.Length > 400000)
                {
                    // ����������� �� ������ PlayerPrefs - �� ������ 1 �� � Web �����. ��������� �� ������������ � ����, ���� ��������� ��������� �������� ����� ��� Unicode (x2 �������)
                    Debug.LogError("Localization size might be too big to save to player prefs: " + base64Data.Length);
                }

                // ������� ������� ������ ������, ����� �� ��������� ������ ������� ��� ������.
                PlayerPrefs.DeleteKey(PrefsDataKey);

                PlayerPrefs.SetString(PrefsDataKey, base64Data);
                PlayerPrefs.SetInt(PrefsVersionKey, CurrentVersion);

                PlayerPrefs.Save();

                Debug.LogFormat("Localization updated to version {0}. Size: {1}", version, base64Data.Length);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Failed to update localization to version {0}. Client version: {1}. Message: {2}", version, ""/*PHPNetwork.NetworkVersion*/, ex.Message);
                Debug.LogException(ex);

                // ������� ���������� � ����� �����������, ��������� ���������.
                PlayerPrefs.DeleteKey(PrefsVersionKey);
                PlayerPrefs.DeleteKey(PrefsDataKey);

                ReloadLocalization(forceFromResources: true);
            }
        }
#endif

        public static bool HasKey(string id)
        {
            return Instance.Data.ContainsKey(id);
        }

        public static string GetText(string id, string targetLocale = null, bool mustExist = true)
        {
            return Instance.GetLocalizationText(id, targetLocale, mustExist);
        }

        private string GetLocalizationText(string id, string targetLocale, bool mustExist = true)
        {
            if (Data == null)
            {
                Debug.LogError("GetLocalizationText: Localization not loaded");
                return "[n/l]" + id;
            }

            targetLocale = targetLocale ?? CurrentLocale;

            if (Data.ContainsKey(id))
            {
                string text = Data[id].textLocals[targetLocale];
                if (text.Length == 0)
                {
#if UNITY_EDITOR
                    if(!id.StartsWith("Timer"))
                        Debug.LogWarningFormat("Localization: Missing {0} key for {1} locale. Using default", id, targetLocale);
#endif
                    return Data[id].textLocals[DefaultLocale];
                }
                else
                {
                    return text;
                }
            }

#if !LIVE_OPS_IMPORTER
            if (mustExist)
                Debug.LogErrorFormat("Localization key '{0}' not found. Target locale: {1}", id, targetLocale);
#endif

            return "[n/l] " + id;
        }

        static public LocalizationText[] GetLocalization()
        {
            var texts = new LocalizationText[Instance.Data.Count];
            var i = 0;
            foreach(var kvp in Instance.Data)
                texts[i++] = kvp.Value;
            return texts;
        }

        public static bool HasArabicLetter(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (IsArabicLetter(text[i]))
                    return true;
            }

            return false;
        }

        public static bool HasArabicLetter(StringBuilder text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (IsArabicLetter(text[i]))
                    return true;
            }

            return false;
        }

        public static bool IsArabicLetter(char ch)
        {
            return (ch >= 0x0600 && ch <= 0x06FF) ||
                   (ch >= 0x0750 && ch <= 0x077F) ||
                   (ch >= 0xFB50 && ch <= 0xFDFF) ||
                   (ch >= 0xFE70 && ch <= 0xFEFF);
        }
  
        public static string GetDeviceCountry()
        {
            return GetDeviceLocalization();
        }

        public static string GetDeviceLocalization()
        {
            return Instance.CurrentLocale.ToUpper();
        }
    }
}