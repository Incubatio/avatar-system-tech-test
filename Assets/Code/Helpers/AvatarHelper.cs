using System.Linq;
using UnityEngine;

public static class AvatarHelper
{
    public static SkinnedMeshRenderer GetSkinMeshRenderer(GameObject pParent)
    {
        var skinnedMeshRenderers = pParent.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderers.Length < 1)
            Debug.LogError("Requires at least one skinned mesh renderer in the FBX");

        if (skinnedMeshRenderers.Length > 1)
            Debug.LogWarning("Several SkinMeshRendererFound, using the first one");
        
        return skinnedMeshRenderers[0];
    }
    
    public static void ReBone(SkinnedMeshRenderer pSkinnedMeshRenderer, Transform pBoneRoot)
    {
        pSkinnedMeshRenderer.rootBone = pBoneRoot;

        var MeshBones = pSkinnedMeshRenderer.bones;

        Transform[] allBones = pBoneRoot.GetComponentsInChildren<Transform>();
        var bones = new Transform[MeshBones.Length];
        for (var i = 0; i < MeshBones.Length; i++)
        {
            bones[i] = allBones.First(name => name.name == MeshBones[i].name);
        }

        pSkinnedMeshRenderer.bones = bones;
    }
    
    public static void ApplyConfig(AvatarAssembler pData, AvatarConfig pConfig)
    {
        AvatarHelper.ClearParts(pData);
        
        if (pConfig.Top)
        {
            var top = Object.Instantiate(pConfig.Top, pData.TopParent);
            var skinnedMeshRenderer = AvatarHelper.GetSkinMeshRenderer(top);
            AvatarHelper.ReBone(skinnedMeshRenderer, pData.BoneRoot);
            AvatarHelper.ApplyMaterial(skinnedMeshRenderer, pConfig.Material1);
        }

        if (pConfig.Bottom)
        {
            var bottom = Object.Instantiate(pConfig.Bottom, pData.BottomParent);
            var skinnedMeshRenderer = AvatarHelper.GetSkinMeshRenderer(bottom);
            AvatarHelper.ReBone(skinnedMeshRenderer, pData.BoneRoot);
            AvatarHelper.ApplyMaterial(skinnedMeshRenderer, pConfig.Material2);
        }
        if (pConfig.Hair) Object.Instantiate(pConfig.Hair, pData.HairParent);
        if (pConfig.Accessory) Object.Instantiate(pConfig.Accessory, pData.AccessoryParent);
        
    }

    public static void ApplyMaterial(Renderer pRenderer, Material pMaterial)
    {
        if (pMaterial == null)
            return;
        
        pRenderer.GetComponent<Renderer>().sharedMaterial = pMaterial;
    }

    public static void ClearParts(AvatarAssembler pData)
    {
        Transform[] parents =  { pData.HairParent, pData.TopParent, pData.BoneRoot, pData.AccessoryParent};
        foreach (Transform parent in parents)
            foreach (Transform child in parent)
                if (child.name.Contains("mixamorig") == false)
                // TODO: check what it is instead of what is not
                //if (child.name.Start_With("avatar_")) 
                    Object.Destroy(child.gameObject);
    }
}