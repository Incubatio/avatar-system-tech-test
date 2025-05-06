using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using System.Text.Json;

public class MainUIController
{
    
    private GameObject _LocalPlayer;
    private VisualElement _RootUI;
    private AvatarSet _Avatar;
    public string SavedAvatarPath;
    public JsonSerializerOptions JsonSerializerOptions;

    public MainUIController(GameObject pLocalPlayer, VisualElement pRootUI)
    {
        _LocalPlayer = pLocalPlayer;
        _RootUI = pRootUI;
        
        // TODO: Folder initialization should be done somewhere else
        JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
        SavedAvatarPath = Application.persistentDataPath + "/SavedAvatarSet.json";
        var directory = Path.GetDirectoryName(SavedAvatarPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    public void BindInteractions()
    {
        _RootUI.Q<Button>(UIHelper.HAIR_BTN).clicked += OnChangeHair;
        _RootUI.Q<Button>(UIHelper.ACCESSORY_BTN).clicked += OnChangeAccessory;
        _RootUI.Q<Button>(UIHelper.TOP_BTN).clicked += OnChangeTop;
        _RootUI.Q<Button>(UIHelper.BOTTOM_BTN).clicked += OnChangeBottom;
        _RootUI.Q<Button>(UIHelper.LOAD_BTN).clicked += OnAvatarLoad;
        _RootUI.Q<Button>(UIHelper.SAVE_BTN).clicked += OnAvatarSave;

        var dropdown = _RootUI.Q<DropdownField>(UIHelper.MATERIAL1_DROPDOWN);
        UIHelper.InitializeDropdown(dropdown, Configs.Avatar.Materials);
        dropdown.RegisterValueChangedCallback(OnChangeMaterial1);

        dropdown = _RootUI.Q<DropdownField>(UIHelper.MATERIAL2_DROPDOWN);
        UIHelper.InitializeDropdown(dropdown, Configs.Avatar.Materials);
        dropdown.RegisterValueChangedCallback(OnChangeMaterial2);
    }

    public void UnbindInteractions()
    {
        _RootUI.Q<Button>(UIHelper.HAIR_BTN).clicked -= OnChangeHair;
        _RootUI.Q<Button>(UIHelper.ACCESSORY_BTN).clicked -= OnChangeAccessory;
        _RootUI.Q<Button>(UIHelper.TOP_BTN).clicked -= OnChangeTop;
        _RootUI.Q<Button>(UIHelper.BOTTOM_BTN).clicked -= OnChangeBottom;
        _RootUI.Q<Button>(UIHelper.LOAD_BTN).clicked -= OnAvatarLoad;
        _RootUI.Q<Button>(UIHelper.SAVE_BTN).clicked -= OnAvatarSave;
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL1_DROPDOWN).UnregisterValueChangedCallback(OnChangeMaterial1);
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL2_DROPDOWN).UnregisterValueChangedCallback(OnChangeMaterial2);
    }


    private void OnAvatarSave()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        var avatarSet = new AvatarSetProxy();
        Func<Transform, string> getPartName = parent => SceneDataSourceHelper.GetCurrentPart(parent).name;

        avatarSet.Hair = getPartName(skinnedBase.HairParent);
        avatarSet.Bottom = getPartName(skinnedBase.BottomParent); 
        avatarSet.Top =  getPartName(skinnedBase.TopParent);
        avatarSet.Accessory = getPartName(skinnedBase.AccessoryParent);
        avatarSet.Material1 = _RootUI.Q<DropdownField>(UIHelper.MATERIAL1_DROPDOWN).value;
        avatarSet.Material2 = _RootUI.Q<DropdownField>(UIHelper.MATERIAL2_DROPDOWN).value;
        
        var rawJson = JsonSerializer.Serialize(avatarSet, JsonSerializerOptions);
        File.WriteAllText(SavedAvatarPath, rawJson);
        Debug.Log(SavedAvatarPath);
    }
    
    private void OnAvatarLoad()
    {
        var rawJson = File.ReadAllText(SavedAvatarPath);
        var avatarSetProxy = JsonSerializer.Deserialize<AvatarSetProxy>(rawJson, JsonSerializerOptions);
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();

        void updatePart(string partName, Transform parent, GameObject[] parts) =>
            _UpdatePart(parts.First(part => part.name == partName), parent);
        void updateSkinnedPart(string partName, Transform parent, GameObject[] parts) =>
            _UpdateSkinnedPart(parts.First(part => part.name == partName), skinnedBase.BoneRoot, parent);

        updatePart(avatarSetProxy.Hair, skinnedBase.HairParent, Configs.Avatar.HairParts);
        updatePart(avatarSetProxy.Accessory, skinnedBase.AccessoryParent, Configs.Avatar.AccessoryParts);
        updateSkinnedPart(avatarSetProxy.Bottom, skinnedBase.BottomParent, Configs.Avatar.BottomParts);
        updateSkinnedPart(avatarSetProxy.Top, skinnedBase.TopParent, Configs.Avatar.TopParts);
        AvatarHelper.ChangeMaterial(avatarSetProxy.Material1, skinnedBase.TopParent);
        AvatarHelper.ChangeMaterial(avatarSetProxy.Material2, skinnedBase.BottomParent);
        
        UpdateButtonLabels();
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL1_DROPDOWN).value = avatarSetProxy.Material1;
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL2_DROPDOWN).value = avatarSetProxy.Material2;
    }

    public void UpdateButtonLabels()
    {
        Func<string, string[]> __format = str => str.Replace("00_", "").Split('_');
        
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();

        (Transform parent, string uiKey)[] parts = {
            (skinnedBase.HairParent, UIHelper.HAIR_BTN),
            (skinnedBase.TopParent, UIHelper.TOP_BTN),
            (skinnedBase.BottomParent, UIHelper.BOTTOM_BTN),
            (skinnedBase.AccessoryParent, UIHelper.ACCESSORY_BTN)
        };

        foreach (var (parent, uiKey) in parts)
        {
            var part = SceneDataSourceHelper.GetCurrentPart(parent);
            var name = __format(part.name);
            _RootUI.Q<Button>(uiKey).text = $"- {name[0]} - \n({name[1]})";
        }
    }

    public void InitDropdownValues()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        var topPart = SceneDataSourceHelper.GetCurrentPart(skinnedBase.TopParent);
        var topRenderer = SkinnedMeshHelper.GetRenderer(topPart);
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL1_DROPDOWN).value = topRenderer.sharedMaterial.name;
        
        var bottomPart = SceneDataSourceHelper.GetCurrentPart(skinnedBase.BottomParent);
        var bottomRenderer = SkinnedMeshHelper.GetRenderer(bottomPart);
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL2_DROPDOWN).value = bottomRenderer.sharedMaterial.name;
    }


    private void _UpdatePart(GameObject pPartRef, Transform pParent)
    {
        var previousPart = SceneDataSourceHelper.GetCurrentPart(pParent);
        if( previousPart.name == pPartRef.name)
            return;
        var nextPart = Object.Instantiate(pPartRef, pParent);
        AvatarHelper.UpdatePart(nextPart);
        Object.DestroyImmediate(previousPart);
    }

    private void _UpdateSkinnedPart(GameObject pPartRef, Transform pBoneRoot, Transform pParent)
    {
        var previousPart = SceneDataSourceHelper.GetCurrentPart(pParent);
        if( previousPart.name == pPartRef.name)
            return;
        var nextPart = Object.Instantiate(pPartRef, pParent);
        
        var previousMaterial = SkinnedMeshHelper.GetRenderer(previousPart).material;
        AvatarHelper.UpdateSkinnedPart(nextPart, pBoneRoot, previousMaterial);
        Object.DestroyImmediate(previousPart);
    }
    private void _NextPart(Transform pParent, GameObject[] pParts)
    {
        var previousPart = SceneDataSourceHelper.GetCurrentPart(pParent);
        _UpdatePart(AvatarHelper.GetNextPartRef(previousPart, pParts), pParent);
    }
    private void _NextSkinnedPart(Transform pParent, Transform pBoneRoot, GameObject[] pParts)
    {
        var previousPart = SceneDataSourceHelper.GetCurrentPart(pParent);
        var nextPartRef = AvatarHelper.GetNextPartRef(previousPart, pParts);
        _UpdateSkinnedPart(nextPartRef, pBoneRoot, pParent);
    }
    
    public void OnChangeHair()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _NextPart(skinnedBase.HairParent, Configs.Avatar.HairParts );
        UpdateButtonLabels();
    }
    public void OnChangeAccessory()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _NextPart(skinnedBase.AccessoryParent, Configs.Avatar.AccessoryParts );
        UpdateButtonLabels();
    }

    public void OnChangeTop()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _NextSkinnedPart(skinnedBase.TopParent, skinnedBase.BoneRoot, Configs.Avatar.TopParts);
        UpdateButtonLabels();
    }

    public void OnChangeBottom()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _NextSkinnedPart(skinnedBase.BottomParent, skinnedBase.BoneRoot, Configs.Avatar.BottomParts);
        UpdateButtonLabels();
    }

    public void OnChangeMaterial1(ChangeEvent<string> evt)
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        AvatarHelper.ChangeMaterial(evt.newValue, skinnedBase.TopParent);
    }

    public void OnChangeMaterial2(ChangeEvent<string> evt)
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        AvatarHelper.ChangeMaterial(evt.newValue, skinnedBase.BottomParent);
    }
}