using UnityEngine;

[CreateAssetMenu(fileName = "AvatarPartConfig", menuName = "Avatar/Avatar parts Config")]
public class AvatarSystemConfig : ScriptableObject
{
    public GameObject[] HairParts;
    public GameObject[] TopParts;
    public GameObject[] BottomParts;
    public GameObject[] AccessoryParts;
    public GameObject[] Materials;
}
