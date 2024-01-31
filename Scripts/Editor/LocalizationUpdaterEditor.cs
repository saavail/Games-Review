using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Localization;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LocalizationUpdater))]
    public class LocalizationUpdaterEditor : UnityEditor.Editor
    {
        private LocalizationUpdater lUpdater;

        private string GetResourcePath()
        {
            return Path.Combine(Application.dataPath, Path.Combine(lUpdater.resourcesPath, lUpdater.resourcesLocalizationFileName));
        }

        private string GetVersionPath()
        {
            return Path.Combine(Application.dataPath, Path.Combine(lUpdater.resourcesPath, lUpdater.resourcesVersionFileName));
        }

        private int GetCurrentVersion()
        {
            int version = 1;
            string versionPath = GetVersionPath();
            if (File.Exists(versionPath))
                version = int.Parse(File.ReadAllText(versionPath));

            return version;
        }

        private void UpdateLocalization(bool removeOldKeys)
        {
            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                using (WWW www = new WWW(string.Format("https://docs.google.com/spreadsheets/d/{0}/export?gid={1}&format=tsv", lUpdater.docId, lUpdater.gid)))
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    while (!www.isDone) ;

                    int version = GetCurrentVersion();
                    version++;
                    File.WriteAllText(GetVersionPath(), version.ToString());

                    File.WriteAllBytes(GetResourcePath(), www.bytes);

                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                    UpdateEnumEntries(removeOldKeys);

                    if (www.error != null)
                        Debug.LogError("Error updating localization: " + www.error);
                    else
                        Debug.Log("Localization Updated, version: " + version);
                }
            }
            finally
            {
                //BalanceImporter.ClearSingletons();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            lUpdater = target as LocalizationUpdater;

            EditorGUILayout.Separator();

            if (lUpdater.docId == "")
            {
                EditorGUILayout.HelpBox("\"Doc Id\" is key of spreadsheet with localization:\n" +
                                        "https://docs.google.com/spreadsheet/ccc?key=[Doc id]\n", MessageType.Warning);
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Update now available only when app is stopped.", MessageType.Info);
                return;
            }

            var prevBgColor = GUI.backgroundColor;

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Open Localization Document", GUILayout.Height(30)))
            {
                Application.OpenURL(String.Format("https://docs.google.com/spreadsheets/d/{0}/edit#gid={1}", lUpdater.docId, lUpdater.gid));
            }

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Update Localization"))
            {
                UpdateLocalization(false);
            }

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Update Localization\n And Remove Old Keys"))
            {
                UpdateLocalization(true);
            }

            GUI.backgroundColor = prevBgColor;

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        #region LocalizationStringsUpdater
        private const string token_enum_begin = "//enum_keys_begin";
        private const string token_enum_end = "///enum_keys_end";
        private const string token_getters_begin = "//enum_getters_begin";
        private const string token_getters_end = "//enum_getters_end";

        private static string _localizationStringsFilePath = null;

        private string LocalizationStringsFilePath
        {
            get
            {
                if (_localizationStringsFilePath == null)
                    _localizationStringsFilePath = Application.dataPath + "/" + lUpdater.localizationStringsPath;

                return _localizationStringsFilePath;
            }
        }        

        public struct LocalizationEnumEntry
        {
            public string key;
            public int value;
        }

        private List<LocalizationEnumEntry> GetLocalizationEnumEntries()
        {
            List<LocalizationEnumEntry> enumEntries = new List<LocalizationEnumEntry>();

            string fileContents = "";
            if (File.Exists(LocalizationStringsFilePath))
            {
                fileContents = File.ReadAllText(LocalizationStringsFilePath);
            }

            int enum_begin_pos = fileContents.IndexOf(token_enum_begin);
            int enum_end_pos = fileContents.IndexOf(token_enum_end);
            if (enum_begin_pos != -1 && enum_end_pos != -1 && enum_end_pos > enum_begin_pos)
            {
                enum_begin_pos += token_enum_begin.Length + 1;
                string enum_strings_raw = fileContents.Substring(enum_begin_pos, enum_end_pos - enum_begin_pos);
                List<string> enum_strings = enum_strings_raw
                    .Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Replace("\r", "").Replace("\t", "").Replace(" ", "").Replace(",", ""))
                    .ToList();
                enum_strings.RemoveAll(a => a.Length == 0);

                foreach (string curEnumLine in enum_strings)
                {
                    LocalizationEnumEntry curEntry = new LocalizationEnumEntry();
                    string[] splittedLine = curEnumLine.Split(new string[] { "=" }, System.StringSplitOptions.RemoveEmptyEntries);
                    curEntry.key = splittedLine[0];
                    curEntry.value = int.Parse(splittedLine[1]);
                    enumEntries.Add(curEntry);
                }
            }
            return enumEntries;
        }

        private void UpdateEnumEntries(bool removeOldKeys)
        {
            List<LocalizationEnumEntry> entries = GetLocalizationEnumEntries();

            LocalizationManager.Instance.ReloadLocalization(forceFromResources: true);
            List<string> newLocKeys = LocalizationManager.Instance.Data.Keys.ToList();

            //old keys?
            List<string> oldKeys = entries.Where(a => !newLocKeys.Contains(a.key)).Select(b => b.key).ToList();
            if (oldKeys.Count > 0)
            {
                string log_oldKeys = "Old keys:\n";
                oldKeys.ForEach(a => log_oldKeys += a + "\n");
                Debug.LogWarning(log_oldKeys);

                if (removeOldKeys)
                {
                    entries.RemoveAll(a => oldKeys.Contains(a.key));
                    Debug.LogWarning("Old keys removed");
                }
                else
                {
                    Debug.LogWarning("Old keys not removed");
                }
            }
            ///

            newLocKeys.RemoveAll(a => entries.Any(b => b.key == a));

            int maxEnumValue = entries.Select(a => a.value).DefaultIfEmpty(1).Max();
            newLocKeys.ForEach(a => entries.Add(new LocalizationEnumEntry() { key = a, value = ++maxEnumValue }));

            string enumValuesString = "";
            string gettersString = "";
            foreach (LocalizationEnumEntry curEntry in entries)
            {
                string curKey = curEntry.key.Replace(" ", "").Replace("-", "").Replace("'", "");
                enumValuesString += string.Format("\t\t{0} = {1},\n", curKey, curEntry.value);

                gettersString += string.Format("\t\tpublic static string {0} => Keys.{0}.ToString().Localized();\n", curKey);
            }

            File.WriteAllText(LocalizationStringsFilePath,
                LocStringsTemplate
                    .Replace("{enum_keys}", enumValuesString)
                    .Replace("{getters}", gettersString));


            LocalizationManager.Instance.ReloadLocalization(forceFromResources: true);

            AssetDatabase.Refresh();
        }

        private const string LocStringsTemplate =
            @"
/******
do not manually edit this file, use LocalizationUpdaterEditor::UpdateEnumEntries()
******/

using Localization;

public static class Strings
{
	public enum Keys
	{
		//enum_keys_begin
{enum_keys}
		///enum_keys_end
	}
	
	//getters_begin
{getters}
	///getters_end
}
";
        #endregion
    }
}