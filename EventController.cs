using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventController : MonoBehaviour {
    public GameObject eventArea;
    public Transform camPos;
    public GameObject[] enemy;
    public Transform enemyPos;
    public GameObject[] chest;
    public Transform chestPos;
    public GameObject[] herb;
    public Transform herbPos;
    public GameObject[] background;
    public Transform backgroundPos;
    private List<GameObject> instances = new List<GameObject>();

//Spawn background and event prefabs
    public void Spawn(EventTypes myEvent){
        CreateInstance(background, backgroundPos);
        eventArea.GetComponent<Area>().ResetReward();
        if(myEvent == EventTypes.Enemy){
            CreateInstance(enemy, enemyPos);
        } else if(myEvent == EventTypes.Chest){
            CreateInstance(chest, chestPos);
        } else if(myEvent == EventTypes.Gather){
            CreateInstance(herb, herbPos);
        }
    }

//Delete instantiated prefabs
    public void Despawn(){
        foreach(GameObject instance in instances){
            Destroy(instance);
        }
        instances.Clear();
    }

//Instantiate a random prefab and set it in the hierarchy
    public void CreateInstance(GameObject[] prefab, Transform trans){
        int rand = Random.Range(0, prefab.Length);  
        GameObject instance = (GameObject)Instantiate(prefab[rand], trans.position, trans.rotation);
        instance.transform.SetParent(trans.parent);
        instances.Add(instance);
    }
	
    public enum EventTypes{
        Enemy,
        Chest,
        Gather
    }
}
