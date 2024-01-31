using System.IO;
using System.Linq;
using Core;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities.Editor
{
    [InitializeOnLoad]
    public class DataCollectableSaver : AssetsModifiedProcessor
    {
        static DataCollectableSaver()
        {
            EditorSceneManager.sceneSaving += EditorSceneManager_sceneSaving;
        }

        private static void EditorSceneManager_sceneSaving(Scene scene, string path)
        {
            foreach (IDataCollectable dataCollectable in scene.GetRootGameObjects()
                         .SelectMany(i => i.GetComponentsInChildren<IDataCollectable>()))
            {
                dataCollectable.CollectData();
            }

            AssetDatabase.SaveAssets();
        }

        protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets,
            AssetMoveInfo[] movedAssets)
        {
            Collect(changedAssets);
            AssetDatabase.SaveAssets();
        }

        private void Collect(string[] paths)
        {
            foreach (var path in paths)
            {
                if (AssetDatabase.LoadAssetAtPath<Object>(path) is not GameObject gameObject)
                    continue;

                foreach (IDataCollectable dataCollectable in gameObject.GetComponentsInChildren<IDataCollectable>())
                {
                    dataCollectable.CollectData();
                }
            }
        }
    }
}