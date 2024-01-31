using UnityEditor;
using Utilities;

namespace Editor
{
    [CustomEditor(typeof(PrettyButton))]
    public class PrettyButtonEditor : UnityEditor.UI.ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            SerializedObject so = new SerializedObject(target);
        
            SerializedProperty stringsProperty = so.FindProperty("_ignoreRenderers");
            EditorGUILayout.PropertyField(stringsProperty, true); 
        
            SerializedProperty renderers = so.FindProperty("_renderers");
            EditorGUILayout.PropertyField(renderers, true); 

            SerializedProperty bounceAnimation = so.FindProperty("_haveBounceAnimation");
            EditorGUILayout.PropertyField(bounceAnimation, true); 
        
            if (bounceAnimation.boolValue)
            {
                SerializedProperty scaleFactor = so.FindProperty("_targetScaleFactor");
                SerializedProperty animationTime = so.FindProperty("_animationTime");
                EditorGUILayout.PropertyField(scaleFactor, true);
                EditorGUILayout.PropertyField(animationTime, true);
            }
        
            EditorGUILayout.Space(10f);

            so.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}