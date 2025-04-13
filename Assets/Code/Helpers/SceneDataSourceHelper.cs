using UnityEngine;
public class SceneDataSourceHelper
{
    public static void ClearParts(Transform pParent)
    {
        foreach (Transform child in pParent)
            if (child.name.Contains("mixamorig") == false)
            // TODO: check what it is instead of what is not
            //if (child.name.Start_With("avatar_")) 
                Object.DestroyImmediate(child.gameObject);
    }

    public static GameObject GetCurrentPart(Transform pParent)
    {
        foreach (Transform child in pParent)
            if (child.name.Contains("mixamorig") == false)
                return child.gameObject;

        return null;
    }
}