using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PropsDatabase))]
    public class PropsDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PropsDatabase database = (PropsDatabase)target;

            GUILayout.Space(10);
            if (GUILayout.Button("Rebuild Dictionary from List"))
            {
                database.RebuildDictionaryFromList();
                EditorUtility.SetDirty(database);
            }
        }
    }
}
