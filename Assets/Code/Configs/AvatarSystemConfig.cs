using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AvatarPartConfig", menuName = "Avatar/Avatar parts Config")]
public class AvatarSystemConfig : ScriptableObject
{
    [FormerlySerializedAs("AvatarBases")] public GameObject[] Bases;
    
    public GameObject[] HairParts;
    public GameObject[] TopParts;
    public GameObject[] BottomParts;
    public GameObject[] AccessoryParts;
    public Material[] Materials;
}
