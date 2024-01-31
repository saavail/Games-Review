using Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    [InitializeOnLoad]
    public static class SaveScenePostProcessor
    {
        static SaveScenePostProcessor()
        {
            EditorSceneManager.sceneSaved += EditorSceneManager_SceneSaved;
        }

        private static void EditorSceneManager_SceneSaved(Scene scene)
        {
            CollectData(scene);
        }

        private static void CollectData(Scene scene)
        {
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                foreach (IDataCollectable dataCollectable in gameObject.GetComponentsInChildren<IDataCollectable>())
                {
                    dataCollectable.CollectData();
                }
            }
        }
    }
}