using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMono : MonoBehaviour
{
    public GameObject mainActor; 
    public AvatarConfig[] avatarSets;
    
    
    private List<GameObject> _Actors;
    void Awake()
    {
        _Actors = new List<GameObject>();

        var currentScene = SceneManager.GetActiveScene();
        var gameObjects = currentScene.GetRootGameObjects();
        
        //var avatar = gameObjects.First(go => go.name == "X Bot").GetComponent<AvatarAssembler>();
        //avatar.ApplyConfig(avatarSets[0]);
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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
