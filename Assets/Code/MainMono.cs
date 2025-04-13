using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMono : MonoBehaviour
{
    [Header("Avatar System")]
    public AvatarAssembler[] AvatarSets;
    public AvatarSystemConfig AvatarConfig;
    
    private List<GameObject> _Actors;
    private MainUIController _MainUIController;
    

    void Awake()
    {
        Configs.Avatar = AvatarConfig;
        
        _Actors = new List<GameObject>();

        var currentScene = SceneManager.GetActiveScene();
        var gameObjects = currentScene.GetRootGameObjects();
        var rootUI = gameObjects.First(go => go.name == UIHelper.UI_DOCUMENT).GetComponent<UIDocument>().rootVisualElement;
        var localPlayer = gameObjects.First(go => go.name == "base_remy");
        
        var uiController = new MainUIController(localPlayer, rootUI);
        uiController.BindInteractions();
        
        var skinMeshComponent = localPlayer.GetComponent<SkinnedBaseComponent>();
        AvatarHelper.ApplyConfig(skinMeshComponent, AvatarSets[0]);
        
        
        localPlayer.GetComponent<Animator>().Play("Walking");
        
        //uiController.UpdateButtonLabels();
        /*
        var actorParent= gameObjects.First(go => go.name == "Actors");
        var spawnParent= gameObjects.First(go => go.name == "Spawns").transform;

        string[] animations = { "Idle", "Walking", "Running" };
        var i = 0;
        foreach (Transform child in spawnParent)
        {
            var obj = GameObject.Instantiate(mainActor, child.position, child.rotation, actorParent.transform);
            obj.GetComponent<Animator>().Play(animations[i++ % animations.Length]);
            var avatar = obj.GetComponent<AvatarAssembler>();
            avatar.ApplyConfig(avatarSets[i % avatarSets.Length]);
            _Actors.Add(obj);
        }
        //avatar.ApplyConfig(avatarSets[0]);
        */

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
