using UnityEngine;
using System.Collections;
using System.Linq;

public class EventCollisions : MonoBehaviour {
    public GameObject eventArea;
    public GameObject player;
    private int eventReward;
    private bool exitHitBool;
    private bool enemyHitBool;

    public void ProcessCollision(Collider2D hitCollider){
        player.GetComponent<Player>().WaypointReached();
        switch(hitCollider.tag){
            case "Exit":
                hitCollider.gameObject.GetComponent<Warp>().WarpToTarget(player);
                // This is to avoid multiple calls
                if(!exitHitBool){
                    Debug.Log("exitHit");
                    exitHitBool = true;
                    StartCoroutine(ResetHitBools(2f)); //reset the exitHit boolean, so it can be used again
                    ExitCollision();
                }
                break;
            case "Enemy":
                //This makes the player slow down
//                player.GetComponent<Player>().SetFunctionState(1);
                // This is to avoid multiple calls
                if(!enemyHitBool){
                    enemyHitBool = true;
                    StartCoroutine(ResetHitBools(2f)); //reset the enemyHit boolean, so it can be used again
                    EnemyCollision(hitCollider.gameObject);
                }
                //Sets the player's attacked bool to yes
                player.GetComponent<Player>().TriggerAnimation(Player.EventAnimation.Attack);
                break;
            case "Chest":
                ChestCollision(hitCollider.gameObject);
                player.GetComponent<Player>().TriggerAnimation(Player.EventAnimation.Celebrate);
                break;
            case "Herb":
                HerbCollision(hitCollider.gameObject);
                player.GetComponent<Player>().TriggerAnimation(Player.EventAnimation.Celebrate);
                break;
            default:
                break;
        }
    }

    public void EnemyCollision(GameObject enemy){
        eventReward = eventArea.GetComponent<Area>().rewardLevel;
        enemy.GetComponent<Enemy>().EnemyHit();
        player.GetComponent<PlayerStats>().DamagePlayer(eventReward);
        player.GetComponent<PlayerStats>().IncreaseExp(eventReward);
    }

    public void ChestCollision(GameObject chest){
        eventReward = eventArea.GetComponent<Area>().rewardLevel;
        chest.SetActive(false);
        player.GetComponent<PlayerStats>().IncreaseGold(eventReward);
    }

    public void HerbCollision(GameObject herb){
        eventReward = eventArea.GetComponent<Area>().rewardLevel;
        herb.SetActive(false);
        player.GetComponent<PlayerStats>().IncreasePotions(eventReward);
    }

    public void ExitCollision(){
        EventController.EventTypes eventType = (EventController.EventTypes)Random.Range(0,3);
        gameObject.GetComponent<EventController>().Spawn(eventType);
    }

    IEnumerator ResetHitBools(float seconds) {
        yield return new WaitForSeconds(seconds);
        enemyHitBool = false;
        exitHitBool = false;
    }
}
