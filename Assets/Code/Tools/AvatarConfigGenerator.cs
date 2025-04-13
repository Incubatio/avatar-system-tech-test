#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class AvatarConfigGenerator
{
    [MenuItem("Avatar/Generate Config")]
    public static void GenerateAvatarPartsConfig()
    {
        var partsConfig = ScriptableObject.CreateInstance<AvatarSystemConfig>();
        string path = "Assets/Generated/AvatarSystemConfig.asset";

        var existingConfig = AssetDatabase.LoadAssetAtPath<AvatarSystemConfig>(path);
        if (existingConfig != null)
            partsConfig = existingConfig;

        partsConfig.Bases = FindAssetsByPrefix<GameObject>("Base_");
        partsConfig.HairParts = FindAssetsByPrefix<GameObject>("Hair_");
        partsConfig.TopParts = FindAssetsByPrefix<GameObject>("Top_");
        partsConfig.BottomParts = FindAssetsByPrefix<GameObject>("Bottom_");
        partsConfig.AccessoryParts = FindAssetsByPrefix<GameObject>("Accessory_");
        partsConfig.Materials = FindAssetsByPrefix<Material>("Material_");

        if (existingConfig == null)
            AssetDatabase.CreateAsset(partsConfig, path);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Avatar Parts Config generated/updated at: " + path);
    }

    private static T[] FindAssetsByPrefix<T>(string pPrefix) where T : Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset != null && asset.name.StartsWith(pPrefix))
                assets.Add(asset);
        }

        return assets.ToArray();
    }
}
#endif
