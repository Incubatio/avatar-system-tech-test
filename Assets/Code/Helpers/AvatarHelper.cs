using UnityEngine;

public static class AvatarHelper
{
    
    public static void ApplyConfig(SkinnedBaseComponent pSkinnedBaseComponent, AvatarAssembler pAvatarSet)
    {
        Transform[] parents =  { pSkinnedBaseComponent.HairParent, pSkinnedBaseComponent.TopParent, 
            pSkinnedBaseComponent.BottomParent, pSkinnedBaseComponent.AccessoryParent };
        foreach (Transform parent in parents)
            SceneDataSourceHelper.ClearParts(parent);

        if (pAvatarSet.Top)
        {
            var top = Object.Instantiate(pAvatarSet.Top, pSkinnedBaseComponent.TopParent);
            UpdateSkinnedPart(top, pSkinnedBaseComponent.BoneRoot, pAvatarSet.Material1);
        }

        if (pAvatarSet.Bottom)
        {
            var bottom = Object.Instantiate(pAvatarSet.Bottom, pSkinnedBaseComponent.BottomParent);
            UpdateSkinnedPart(bottom, pSkinnedBaseComponent.BoneRoot, pAvatarSet.Material2);
        }

        if (pAvatarSet.Hair)
        {
            var hair = Object.Instantiate(pAvatarSet.Hair, pSkinnedBaseComponent.HairParent);
            UpdatePart(hair);
        }

        if (pAvatarSet.Accessory)
        {
            var accessory = Object.Instantiate(pAvatarSet.Accessory, pSkinnedBaseComponent.AccessoryParent);
            UpdatePart(accessory);
        }

    }

    public static void ApplyMaterial(Renderer pRenderer, Material pMaterial)
    {
        if (pMaterial == null)
            return;
        
        pRenderer.GetComponent<Renderer>().sharedMaterial = pMaterial;
    }

    public static void UpdatePart(GameObject pObject ) => pObject.name = pObject.name.Replace("(Clone)", "");

    public static void UpdateSkinnedPart(GameObject pObject, Transform pBoneRoot, Material pMaterial )
    {
        pObject.name = pObject.name.Replace("(Clone)", "");
        var skinnedMeshRenderer = SkinnedMeshHelper.GetRenderer(pObject);
        SkinnedMeshHelper.ReBone(skinnedMeshRenderer, pBoneRoot);
        AvatarHelper.ApplyMaterial(skinnedMeshRenderer, pMaterial);
    }


    public static GameObject GetNextPartRef(GameObject pPreviousPart, GameObject[] pParts)
    {
        var targetIndex = -1;
        if (pPreviousPart != null)
            targetIndex = System.Array.FindIndex(pParts, obj => pPreviousPart.name == obj.name);

        return pParts[(targetIndex + 1) % pParts.Length];
    }
}