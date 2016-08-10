using UnityEngine;
using System.Collections;

public class Warp : MonoBehaviour {
    public GameObject eventController;
    public GameObject area;
    public bool eventScreen;

    public void WarpToTarget(GameObject other){
        Transform camPos = area.GetComponent<Area>().mainCamPos.transform;
        Transform charPos = area.GetComponent<Area>().charEntrancePos.transform;
        other.gameObject.transform.position = new Vector3(charPos.position.x, charPos.position.y, other.gameObject.transform.position.z);
        Camera.main.transform.position = new Vector3(camPos.position.x, camPos.position.y, Camera.main.transform.position.z);
        if(eventScreen){
            eventController.GetComponent<EventController>().Despawn();
        }
    }
}
