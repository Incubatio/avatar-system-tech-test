using System;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class MainUIController
{
    private GameObject _LocalPlayer;
    private VisualElement _RootUI;

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

        //_RootUI.Q<DropdownField>(UIHelper.MATERIAL1_DROPDOWN).RegisterValueChangedCallback() += ChangeMaterial1;
        //_RootUI.Q<DropdownField>(UIHelper.MATERIAL2_DROPDOWN).RegisterValueChangedCallback() += ChangeMaterial2;
    }

    public void UnbindInteractions()
    {
        _RootUI.Q<Button>(UIHelper.HAIR_BTN).clicked -= OnChangeHair;
        _RootUI.Q<Button>(UIHelper.ACCESSORY_BTN).clicked -= OnChangeAccessory;
        _RootUI.Q<Button>(UIHelper.TOP_BTN).clicked -= OnChangeTop;
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
    
}