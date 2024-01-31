using System;
using System.Xml;
using Core;
using Core.Server;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BuildWindow : EditorWindow
    {
        private string _androidManifestPath;

        private const string BuildSettingsDataPath = "Data/BuildSettingsData";
        
        private const string IconPathDev = "GameIcons/DevIcon";
        private const string IconPathProd = "GameIcons/ProdIcon";

        private const string URLSchemesDev = "tezrogamesdev";
        private const string URLSchemesProd = "tezrogames";

        private const string PackageNameDev = "com.TezroDev.TezroGames";
        private const string PackageNameProd = "com.Tezro.TezroGames";

        private void OnEnable()
        {
            _androidManifestPath = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        }

        // Add menu item named "My Window" to the Window menu
        [MenuItem("File/Custom Build Settings")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            var windows = GetWindow(typeof(BuildWindow), false, "Custom Build Settings");
            windows.minSize = new Vector2(250, 250);
            windows.maxSize = new Vector2(350, 350);
            windows.Focus();
        }

        private void OnGUI()
        {
            GUILayout.Label("Build Settings for " + Application.productName, EditorStyles.boldLabel);
            
            GUILayout.BeginVertical();
            GUILayout.Space(30);
            
            
            if (GUILayout.Button("Development Version"))
            {
#if UNITY_ANDROID
                SetVersion(IconPathDev, PackageNameDev, true);
#elif UNITY_IOS
             SetVersion(IconPathDev, PackageNameDev, true);
#endif
            }
            
            GUILayout.BeginVertical();
            GUILayout.Space(30);
            
            if (GUILayout.Button("Production Version"))
            {
#if UNITY_ANDROID
                SetVersion(IconPathProd, PackageNameProd, false);
#elif UNITY_IOS
             SetVersion(IconPathProd, PackageNameProd, false);
#endif
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void SetVersion(string iconPath, string packageName, bool isDevelopment)
        {
            SetIcon(iconPath);
            SetPackageName(packageName);
            SetURLScheme(isDevelopment);
            SetBackendLink(isDevelopment);
        }

        private void SetBackendLink(bool isDevelopment)
        {
            BuildSettingsData buildSettings = Resources.Load<BuildSettingsData>(BuildSettingsDataPath);
            if (buildSettings != null)
            {
                // Change the IsDevelopment value
                buildSettings.IsDevelopment = isDevelopment;

                // Save changes
                EditorUtility.SetDirty(buildSettings);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError("BuildSettings ScriptableObject not found in Resources/Data directory.");
            }
        }

        private void SetURLScheme(bool isDevelopment)
        {
#if UNITY_ANDROID
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(_androidManifestPath);
            
            var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("android", "http://schemas.android.com/apk/res/android");
            var node = xmlDocument.SelectSingleNode("/manifest/application/activity/intent-filter[data/@android:scheme]/data", nsmgr);
            if (node != null)
            {
                XmlNode schemaNode = node.Attributes.GetNamedItem("android:scheme");
                
                if(schemaNode != null)
                {
                    schemaNode.Value = isDevelopment ? URLSchemesDev : URLSchemesProd;
                }
                else {
                    Debug.LogError("Schema node not found in AndroidManifest.xml.");
                    return;
                }
                
                xmlDocument.Save(_androidManifestPath);
            }
#elif UNITY_IOS
             PlayerSettings.iOS.iOSUrlSchemes = new string[] { isDevelopment ? URLSchemesDev : URLSchemesProd };
#endif
        }

        private void SetIcon(string iconPath)
        {
            Texture2D icon = Resources.Load<Texture2D>(iconPath);
            if (icon != null)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { icon });
            }
            else
            {
                Debug.LogError("Cannot load icon at path: " + iconPath);
            }
        }

        private void SetPackageName(string packageName)
        {
#if UNITY_ANDROID
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName);
#elif UNITY_IOS
             PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, packageName);
#endif
        }
    }
}