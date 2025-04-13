using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AvatarSetConfig", menuName = "Avatar/AvatarSet Config")]
public class AvatarAssembler : ScriptableObject
{
    public GameObject Hair;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Accessory;
    public Material Material1;
    public Material Material2;
}


#if UNITY_EDITOR
[CustomEditor(typeof(AvatarAssembler))]
public class AvatarConfigEditor : Editor
{
    private const string BASE_PREFAB_PATH = "Assets/Art/Prefabs/Base_Remy.prefab";
    private const string PARENT_GAMEOBJECT_NAME = "AVATAR_ASSEMBLER_CONTAINER";
    
    private static GameObject _PreviewInstance;
    
    private Animator _PreviewAnimator;
    private bool _IsAnimating = false;
    private float _LastEditorUpdateTime;
    private bool _ConfigurationChanged;

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        //SceneView.duringSceneGui += OnSceneGUI;
        
        if (_PreviewInstance != null)
            _PreviewAnimator = _PreviewInstance.GetComponent<Animator>();

        CreatePreviewCharacter();
    }
    
    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        //SceneView.duringSceneGui -= OnSceneGUI;
        DestroyPreviewCharacter();
        Resources.UnloadUnusedAssets();
    }
    
    private void OnEditorUpdate()
    {
        if (!_IsAnimating || _PreviewInstance == null || _PreviewAnimator == null) 
            return;
            
        // only update every 30 FPS
        if (EditorApplication.timeSinceStartup - _LastEditorUpdateTime < 0.033f)
            return;
        
        float currentTime = (float)EditorApplication.timeSinceStartup;
        float deltaTime = currentTime - _LastEditorUpdateTime;
        _LastEditorUpdateTime = currentTime;
            
        _PreviewAnimator.Update(deltaTime);
            
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.Repaint();
        //SceneView.RepaintAll();
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        
        serializedObject.Update();

        DrawFilteredField("Hair", "Hair_");
        DrawFilteredField("Top", "Top_");
        DrawFilteredField("Bottom", "Bottom_");
        DrawFilteredField("Accessory", "Accessory_");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Material1"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Material2"));
        
        serializedObject.ApplyModifiedProperties();
        
        if (EditorGUI.EndChangeCheck()) 
            _ConfigurationChanged = true;

        EditorGUILayout.Space();
        
        if (_IsAnimating == false && GUILayout.Button("Play Animation"))
            PlayAnimation();

        if (_IsAnimating && GUILayout.Button("Stop Animation"))
            _IsAnimating = false;
        
        if (GUILayout.Button("Clear Preview"))
            DestroyPreviewCharacter();

        if (_ConfigurationChanged && _PreviewInstance != null)
        {
            _ConfigurationChanged = false;
            Debug.Log("Config Updated, refresh avatar");
            AvatarAssembler config = target as AvatarAssembler;
            var skinMeshComponent = _PreviewInstance.GetComponent<SkinnedBaseComponent>();
            AvatarHelper.ApplyConfig(skinMeshComponent, config);
        }
    }
    
    void OnSceneGUI(SceneView sceneView)
    {
        // Check for key presses
        //Event e = Event.current;
        //if (e.type == EventType.KeyDown && e.keyCode == _UpdateKey)
        //if (e.type == EventType.MouseDown && e.button == 0) // Update on mouse click
    }

    void CreatePreviewCharacter()
    {
        DestroyPreviewCharacter();
        AvatarAssembler config = target as AvatarAssembler;

        var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BASE_PREFAB_PATH);
        var parent = new GameObject(PARENT_GAMEOBJECT_NAME);
        parent.hideFlags = HideFlags.DontSave;
        _PreviewInstance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab, parent.transform);
        //previewInstance.hideFlags = HideFlags.DontSave;
        
        var skinMeshComponent = _PreviewInstance.GetComponent<SkinnedBaseComponent>();
        AvatarHelper.ApplyConfig(skinMeshComponent, config);

        _PreviewAnimator =  _PreviewInstance.GetComponent<Animator>();
        PlayAnimation();

        if (SceneView.lastActiveSceneView)
        {
            _PreviewInstance.transform.position = Vector3.zero;
        }
    }

    void PlayAnimation()
    {
        if (_IsAnimating) return;
        _IsAnimating = true;
        _PreviewAnimator.Play("Walking", 0, 0); 
    }

    void DestroyPreviewCharacter()
    {
        if (_PreviewInstance == null)
            return;
        
        var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        var parent = gameObjects.FirstOrDefault(go => go.name == PARENT_GAMEOBJECT_NAME);
        if (parent != null)
            DestroyImmediate(parent);

        _IsAnimating = false;
        
        DestroyImmediate(_PreviewInstance);
        _PreviewInstance = null;
        _PreviewAnimator = null;
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
                        _ConfigurationChanged = true;
                    }
                );
            }
        }

        menu.ShowAsContext();
    }

}
#endif