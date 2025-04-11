using UnityEngine;

[CreateAssetMenu(fileName = "AvatarPart", menuName = "Avatar/Avatar Part")]
public class AvatarPart : ScriptableObject
{
    public string partName;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "AvatarConfig", menuName = "Avatar/Avatar Config")]
public class AvatarConfig : ScriptableObject
{
    public AvatarPart hair;
    public AvatarPart top;
    public AvatarPart bottom;
    public AvatarPart accessory;
}
public class AvatarAssembler : MonoBehaviour
{
    //public Transform partParent;

    public Transform hairParent, topParent, bottomParent, accessoryParent;
    private GameObject currentHair, currentTop, currentBottom, currentAccessory;

    public void ApplyConfig(AvatarConfig config)
    {
        ClearParts();
        
        if (config.hair) currentHair = Instantiate(config.hair.prefab, hairParent);
        //if (config.hair) currentHair = Instantiate(config.hair.prefab, partParent);
        if (config.top) currentTop = Instantiate(config.top.prefab, topParent);
        if (config.bottom) currentBottom = Instantiate(config.bottom.prefab, bottomParent);
        if (config.accessory) currentAccessory = Instantiate(config.accessory.prefab, accessoryParent);
    }

    void ClearParts()
    {
        Transform[] parents =  { hairParent, topParent, bottomParent, accessoryParent};
        foreach (Transform parent in parents )
            foreach (Transform child in parent )
                if (child.name.Contains("mixamorig") == false)
                    Destroy(child.gameObject);
    }
}
