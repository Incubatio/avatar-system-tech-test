using UnityEngine.UIElements;

public static class UIHelper
{
    public const string UI_DOCUMENT = "UIDocument";
    public const string HAIR_BTN = "hair-btn";
    public const string TOP_BTN = "top-btn";
    public const string BOTTOM_BTN = "bottom-btn";
    public const string ACCESSORY_BTN = "accessory-btn";
    public const string MATERIAL1_DROPDOWN = "material1-dropdown";
    public const string MATERIAL2_DROPDOWN = "material2-dropdown";
    
    public static void InitializeDropdown<T>(DropdownField dropdown, T[] pData) where T : UnityEngine.Object
    {
        dropdown.choices.Clear();
        foreach (var material in pData)
            dropdown.choices.Add(material.name);

        if (Configs.Avatar.Materials.Length > 0)
            dropdown.value = Configs.Avatar.Materials[0].name;
    }

}