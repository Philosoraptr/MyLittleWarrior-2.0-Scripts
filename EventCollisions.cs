using UnityEngine;
using System.Collections;
using System.Linq;

public class EventCollisions : MonoBehaviour {
    public GameObject eventArea;
    private int eventReward;

    public void EnemyCollision(GameObject enemy, GameObject player){
        eventReward = eventArea.GetComponent<Area>().rewardLevel;
        enemy.GetComponent<Enemy>().EnemyHit();
        player.GetComponent<PlayerStats>().DamagePlayer(eventReward);
        player.GetComponent<PlayerStats>().IncreaseExp(eventReward);
    }

    public void ChestCollision(GameObject chest, GameObject player){
        eventReward = eventArea.GetComponent<Area>().rewardLevel;
        chest.SetActive(false);
        player.GetComponent<PlayerStats>().IncreaseGold(eventReward);
    }

    public void HerbCollision(GameObject herb, GameObject player){
        eventReward = eventArea.GetComponent<Area>().rewardLevel;
        herb.SetActive(false);
        player.GetComponent<PlayerStats>().IncreasePotions(eventReward);
    }

    public void ExitCollision(){
        EventController.EventTypes eventType = 
            (EventController.EventTypes)Random.Range(0,2);
        gameObject.GetComponent<EventController>().Spawn(eventType);
    }
}
