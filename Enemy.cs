using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
[RequireComponent (typeof (Animator))]
public class Enemy : MonoBehaviour {
    Animator anim;

    void Start(){
        anim = GetComponent<Animator>();
    }

    public void EnemyHit(){
        anim.SetBool("Damaged", true);
        StartCoroutine(Die(0.8f));
    }

    IEnumerator Die(float seconds){
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
