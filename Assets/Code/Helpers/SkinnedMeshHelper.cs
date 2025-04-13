using System.Linq;
using UnityEngine;
public static class SkinnedMeshHelper
{
    public static SkinnedMeshRenderer GetRenderer(GameObject pParent)
    {
        // will break if no skinnedMeshRenderer object
        var skinnedMeshRenderers = pParent.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderers.Length > 1)
            Debug.LogWarning("Several SkinMeshRendererFound, using the first one");
        
        return skinnedMeshRenderers[0];
    }
    
    public static void ReBone(SkinnedMeshRenderer pSkinnedMeshRenderer, Transform pBoneRoot)
    {
        pSkinnedMeshRenderer.rootBone = pBoneRoot;

        var meshBones = pSkinnedMeshRenderer.bones;

        Transform[] allBones = pBoneRoot.GetComponentsInChildren<Transform>();
        var bones = new Transform[meshBones.Length];
        for (var i = 0; i < meshBones.Length; i++)
        {
            var bone = allBones.FirstOrDefault(b => b.name == meshBones[i].name);
            if (bone is null)
            {
                bone = Object.Instantiate(meshBones[i], meshBones[i].position, meshBones[i].rotation, pBoneRoot);
                bone.localPosition = meshBones[i].localPosition;
                bone.localScale =    meshBones[i].localScale;
                bone.localRotation = meshBones[i].localRotation;
                bone.name =          meshBones[i].name;
            }
            bones[i] = bone;

        }
        pSkinnedMeshRenderer.bones = bones;
    }
    
}