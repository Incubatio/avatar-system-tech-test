
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarSet", menuName = "Avatar/AvatarSet")]
public class AvatarSet : ScriptableObject
{
    public GameObject Hair;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject Accessory;
    public Material Material1;
    public Material Material2;
}

public class AvatarSetProxy
{
    public string Hair;
    public string Top;
    public string Bottom;
    public string Accessory;
    public string Material1;
    public string Material2;
}