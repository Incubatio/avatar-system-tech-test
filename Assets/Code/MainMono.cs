using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

public class MainMono : MonoBehaviour
{
    [Header("Avatar System")]
    public AvatarSystemConfig AvatarConfig;
    
    [Header("Graphic Settings")]
    public int MirrorTexResolution = 256;
    public GameObject MirrorSurface;
    
    [Header("GamePlay Settings")]
    public int SpawnNumber = 1;
    
    private Camera _MainCamera;
    private List<GameObject> _Actors;
    private MainUIController _MainUIController;
    
    private Camera _MirrorCamera;
    private RenderTexture _renderTexture;
    private bool _DisableUpdate = true;
    
    void Awake()
    {
        Configs.Avatar = AvatarConfig;
        if (Configs.Avatar == null || Configs.Avatar.Bases == null || Configs.Avatar.Bases.Length < 1)
        {
            Debug.LogError("Missing or corrupted avatar config, use following Unity's Menu: \"Avatar\" > \"Generate Config\".\nMake sure \"Avatar Config\" is assigned in MainMono GameObject in the scene");
            return;
        }

        _Actors = new List<GameObject>();

        var currentScene = SceneManager.GetActiveScene();
        var gameObjects = currentScene.GetRootGameObjects();
        var rootUI = gameObjects.First(go => go.name == UIHelper.UI_DOCUMENT).GetComponent<UIDocument>().rootVisualElement;
        _MainCamera = gameObjects.First(go => go.name == "Main Camera").GetComponent<Camera>();
        _MirrorCamera = gameObjects.First(go => go.name == "Mirror Camera").GetComponent<Camera>();
        
        var actorParent= gameObjects.First(go => go.name == "Actors");
        var spawnParent= gameObjects.First(go => go.name == "Spawns").transform;

        string[] animations = { "Idle", "Walking", "Sprint" };
        var mainActor = Configs.Avatar.Bases[0];
        var i = 0;


        T randPart<T>(T[] array) => array[Random.Range(0, array.Length)];
        var assembler = ScriptableObject.CreateInstance<AvatarSet>();
        
        SpawnNumber = Math.Min(SpawnNumber, spawnParent.childCount);
        var vector3dHalf = new Vector3(.5f, .5f, .5f);
        for (i = 0; i < SpawnNumber; i++)
        {
            var child = spawnParent.transform.GetChild(i);
    
            assembler.Hair      = randPart(Configs.Avatar.HairParts);
            assembler.Top       = randPart(Configs.Avatar.TopParts);
            assembler.Bottom    = randPart(Configs.Avatar.BottomParts);
            assembler.Accessory = randPart(Configs.Avatar.AccessoryParts);
            assembler.Material1 = randPart(Configs.Avatar.Materials);
            assembler.Material2 = randPart(Configs.Avatar.Materials);

            if (i > 0)
            {
                Vector3 direction = Vector3.zero - child.position;
                direction.y = 0;
                child.rotation = Quaternion.LookRotation(direction);
            }

            var actor = GameObject.Instantiate(mainActor, child.position, child.rotation, actorParent.transform);
            if (i > 0 && i < 3)
                actor.transform.localScale = vector3dHalf;
            
    
            AvatarHelper.ApplyConfig(actor.GetComponent<SkinnedBaseComponent>(), assembler);
            //actor.GetComponent<Animator>().Play(animations[i % animations.Length]);
            actor.GetComponent<Animator>().Play(animations[i > 2 ? 0 : 2]);
            
            _Actors.Add(actor);
        }
        
        var localPlayer = _Actors.First();
        localPlayer.GetComponent<Animator>().Play("Walking");
        var uiController = new MainUIController(localPlayer, rootUI);
        uiController.BindInteractions();
        
        uiController.UpdateButtonLabels();
        uiController.InitDropdownValues();
        
        
        
        // Setup Mirror Camera to render on mirror surface
        _renderTexture = new RenderTexture(MirrorTexResolution, MirrorTexResolution, 24);
        
        _MirrorCamera.targetTexture = _renderTexture;
        var mirrorMaterial = MirrorSurface.GetComponent<Renderer>().material;
        mirrorMaterial.mainTexture = _renderTexture;
        mirrorMaterial.mainTextureScale = new Vector2(-1, 1);
        mirrorMaterial.mainTextureOffset = new Vector2(1, 0);
        _DisableUpdate = false;
    }

    void Update()
    {
        if (_DisableUpdate)
            return;
        
        _MirrorCamera.Render(); 
    }
}
