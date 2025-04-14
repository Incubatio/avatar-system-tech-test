using System;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class MainUIController
{
    private GameObject _LocalPlayer;
    private VisualElement _RootUI;
    private AvatarSet _Avatar;

    public MainUIController(GameObject pLocalPlayer, VisualElement pRootUI)
    {
        _LocalPlayer = pLocalPlayer;
        _RootUI = pRootUI;
    }

    public void BindInteractions()
    {
        _RootUI.Q<Button>(UIHelper.HAIR_BTN).clicked += OnChangeHair;
        _RootUI.Q<Button>(UIHelper.ACCESSORY_BTN).clicked += OnChangeAccessory;
        _RootUI.Q<Button>(UIHelper.TOP_BTN).clicked += OnChangeTop;
        _RootUI.Q<Button>(UIHelper.BOTTOM_BTN).clicked += OnChangeBottom;

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
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL1_DROPDOWN).UnregisterValueChangedCallback(OnChangeMaterial1);
        _RootUI.Q<DropdownField>(UIHelper.MATERIAL2_DROPDOWN).UnregisterValueChangedCallback(OnChangeMaterial2);
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

    private void _InstanciateNextPart(Transform pParent, GameObject[] pParts)
    {
        var previousPart = SceneDataSourceHelper.GetCurrentPart(pParent);
        var nextPartRef = AvatarHelper.GetNextPartRef(previousPart, pParts);
        var nextPart = Object.Instantiate(nextPartRef, pParent);
        AvatarHelper.UpdatePart(nextPart);
        Object.DestroyImmediate(previousPart);
        
        UpdateButtonLabels();
    }
    public void OnChangeHair()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _InstanciateNextPart(skinnedBase.HairParent, Configs.Avatar.HairParts );
    }
    public void OnChangeAccessory()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _InstanciateNextPart(skinnedBase.AccessoryParent, Configs.Avatar.AccessoryParts );
    }

    private void _InstanciateNextSkinnedPart(Transform pParent, Transform pBoneRoot, GameObject[] pParts)
    {
        var previousPart = SceneDataSourceHelper.GetCurrentPart(pParent);
        var nextPartRef = AvatarHelper.GetNextPartRef(previousPart, pParts);
        var nextPart = Object.Instantiate(nextPartRef, pParent);
        
        var previousMaterial = SkinnedMeshHelper.GetRenderer(previousPart).material;
        AvatarHelper.UpdateSkinnedPart(nextPart, pBoneRoot, previousMaterial);
        
        Object.DestroyImmediate(previousPart);
        UpdateButtonLabels();
    }

    public void OnChangeTop()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _InstanciateNextSkinnedPart(skinnedBase.TopParent, skinnedBase.BoneRoot, Configs.Avatar.TopParts);
    }

    public void OnChangeBottom()
    {
        var skinnedBase = _LocalPlayer.GetComponent<SkinnedBaseComponent>();
        _InstanciateNextSkinnedPart(skinnedBase.BottomParent, skinnedBase.BoneRoot, Configs.Avatar.BottomParts);
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