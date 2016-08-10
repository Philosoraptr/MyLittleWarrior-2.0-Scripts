using UnityEngine;
using System.Collections;

public class Area : MonoBehaviour {
    public Transform charEntrancePos;
    public Transform mainCamPos;
    public int rewardLevel = 1;

    public void ResetReward(){
        rewardLevel = Random.Range(1, 3);
    }
}
