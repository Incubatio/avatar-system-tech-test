using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AvatarConfig", menuName = "Avatar/Avatar Config")]
public class AvatarConfig : ScriptableObject
{
public GameObject Hair;
public GameObject Top;
public GameObject Bottom;
public GameObject Accessory;
public Material Material1;
public Material Material2;
}

#if UNITY_EDITOR
[CustomEditor(typeof(AvatarConfig))]
public class AvatarConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawFilteredField("Hair", "Hair_");
        DrawFilteredField("Top", "Top_");
        DrawFilteredField("Bottom", "Bottom_");
        DrawFilteredField("Accessory", "Accessory_");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Material1"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Material2"));

        serializedObject.ApplyModifiedProperties();
    }

    void DrawFilteredField(string pPropertyName, string pRequiredPrefix)
    {
        SerializedProperty prop = serializedObject.FindProperty(pPropertyName);
        EditorGUILayout.BeginHorizontal();
        
        GameObject current = prop.objectReferenceValue as GameObject;
        string displayName = current ? current.name : "None";
        if (current && !current.name.StartsWith(pRequiredPrefix))
        {
            displayName += " [INVALID]";
            GUI.color = Color.red;
        }

        Rect pos = EditorGUILayout.GetControlRect();
        pos = EditorGUI.PrefixLabel(pos, new GUIContent(pPropertyName));
        
        if (EditorGUI.DropdownButton(pos, new GUIContent(displayName), FocusType.Keyboard))
            ShowPrefixFilteredMenu(prop, pRequiredPrefix);

        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
    }

    void ShowPrefixFilteredMenu(SerializedProperty pProp, string pPrefix)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("None"), false, () => {
            pProp.objectReferenceValue = null;
            serializedObject.ApplyModifiedProperties();
        });

        string[] guids = AssetDatabase.FindAssets("");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (obj != null && obj.name.StartsWith(pPrefix))
            {
                menu.AddItem(new GUIContent(obj.name), false, () => {
                        pProp.objectReferenceValue = obj;
                        serializedObject.ApplyModifiedProperties();
                    }
                );
            }
        }

        menu.ShowAsContext();
    }
}
#endif