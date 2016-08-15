using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (PlayerStats))]
public class Player : MonoBehaviour {

    public enum MoveState{accelerate, decelerate, pause};

    public float direction = -1.0f;

    float gravity = -20.0f;
    Vector3 velocity;

    Controller2D controller;

    Animator anim;
    bool attack;
    bool attackedRecently;
	
////////////////////////////////////////////////////////////////
    //New movement
    // This is the rate of acceleration after the function "Accelerate()" is called.
    // Higher values will cause the object to reach the "speedLimit" in less time.
    public float acceleration = 0.8f;

    // This is the the amount of velocity retained after the function "Decelerate()" is called.
    // Lower values cause quicker stops. A value of "1.0" will never stop. Values above "1.0" will speed up.
    public float inertia = 0.9f;

    // This is as fast the object is allowed to go.
    public float speedLimit = 10.0f;

    // This is the speed that tells the function "Decelerate()" when to stop moving the object.
    public float minSpeed = 1.0f;

    // This is how long to pause inside "Decelerate()" before activating the function
    // "Accelerate()" to start the script again.
    public float stopTime = 1.0f;

    // This variable "currentSpeed" is the major player for dealing with velocity.
    // The "currentSpeed" is multiplied by the variable "acceleration" to speed up inside the function "Accelerate()".
    // Again, The "currentSpeed" is multiplied by the variable "inertia" to slow
    // things down inside the function "Decelerate()".
    private float currentSpeed = 0.0f;

    // The variable "playerMoveState" controls which function, "Accelerate()" or "Decelerate()",
    // is active.
    MoveState playerMoveState;

    // The next two variables are used to make sure that while the function "Accelerate()" is running,
    // the function "Decelerate()" can not run (as well as the reverse).
    private bool accelerationState;
    private bool decelerateState;

    // This variable will store the "active" target object (the waypoint to move to).
    private Transform waypoint;

    public Transform[] waypoints;

    // This variable keeps track of which Waypoint Object,
    // in the previously mentioned array variable "waypoints", is currently active.
    private int WPindexPointer;

    void Start() {
        controller = GetComponent<Controller2D> ();
        anim = GetComponent<Animator>();
        attack = false;
        attackedRecently = false;
//        functionState = 0;
        playerMoveState = MoveState.accelerate;
    }

    void Update() {
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        //get gravity going
        velocity.y += gravity * Time.deltaTime;
        //Turn off the attack animation boolean on the next update
        if(attack && !attackedRecently) {
            attackedRecently = true;
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
                anim.SetTrigger("Attack");
            }
            //This stops the animation being called repeatedly while the player is colliding with the enemy
            StartCoroutine(ResetBools(5.0f));
        }

        // If functionState variable is currently "0" then run "Accelerate()".
        // Without the "if", "Accelerate()" would run every frame.
        if (playerMoveState == MoveState.accelerate){
            Accelerate();
        }

        // If functionState variable is currently "1" then run "Decelerate()".
        // Without the "if", "Decelerate()" would run every frame.
        if (playerMoveState == MoveState.decelerate){
            StartCoroutine(Decelerate());
        }

        waypoint = waypoints[WPindexPointer]; //Keep the object pointed toward the current Waypoint object.

        //For walk animation
        anim.SetFloat("Speed", velocity.x);
    }

    public void SetAttack(){
        attack = true;
    }

    public void Die(){
        Debug.Log("The Player is dead");
        gameObject.SetActive(false);
    }

    void Accelerate (){
        if (accelerationState == false){
            // Make sure that if Accelerate() is running, Decelerate() can not run.
            accelerationState = true;
            decelerateState = false;
        }

        // Now do the acceleration toward the active waypoint until the "speedLimit" is reached
        currentSpeed = currentSpeed + acceleration * acceleration;
        velocity.x = currentSpeed * direction;
        controller.Move (velocity * Time.deltaTime);

        // When the "speedlimit" is reached or exceeded ...
        if (currentSpeed >= speedLimit){
            // Turn off acceleration and set "currentSpeed" to be the "speedLimit"
            currentSpeed = speedLimit;
        }
    }
        
    public void WaypointReached (){
        // activate "Decelerate()"
        playerMoveState = MoveState.decelerate;

        // change the active waypoint to the next one in the array variable "waypoints".
        WPindexPointer++;

        // When the array variable reaches the end of the list ...
        if (WPindexPointer >= waypoints.Length)
        {
            // ... reset the active waypoint to the first object in the array variable
            // "waypoints" and start from the beginning.
            WPindexPointer = 0;
        }
    }

    IEnumerator Decelerate()
    {
        if (decelerateState == false) //
        {
            // Make sure that if Decelerate() is running, Accelerate() can not run.
            accelerationState = false;
            decelerateState = true;
        }

        // Begin to do the slow down (or speed up if inertia is set above "1.0" in the inspector).
        currentSpeed = currentSpeed * inertia;
        velocity.x = currentSpeed * direction;
        controller.Move (velocity * Time.deltaTime);

        // When the "minSpeed" is reached or exceeded ...
        if (currentSpeed <= minSpeed)
        {
            // ... Stop the movement by setting "currentSpeed to Zero.
            currentSpeed = 0.0f;
            // Wait for the amount of time set in "stopTime" before moving to next waypoint.
            yield return new WaitForSeconds(stopTime);
            // Activate the function "Accelerate()" to move to next waypoint.
            playerMoveState = MoveState.accelerate;
        }
    }

    IEnumerator ResetBools(float seconds) {
        yield return new WaitForSeconds(seconds);
        attackedRecently = false;
        attack = false;
    }

    public void SetPlayerMoveState(MoveState state){
        playerMoveState = state;
    }
}
