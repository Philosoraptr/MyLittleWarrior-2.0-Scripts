using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (PlayerStats))]
public class Player : MonoBehaviour {

    public float moveSpeed = 2.0;
	public float direction = -1.0f;
	
    float gravity = -20.0;
    Vector3 velocity;

    Controller2D controller;

    Animator anim;
	bool attackBool;
	
////////////////////////////////////////////////////////////////
   
    //New movement
    // This is the rate of acceleration after the function "Accelerate()" is called.
    // Higher values will cause the object to reach the "speedLimit" in less time.
    public float acceleration = 0.8f;

    // This is the the amount of velocity retained after the function "Slow()" is called.
    // Lower values cause quicker stops. A value of "1.0" will never stop. Values above "1.0" will speed up.
    public float inertia = 0.9f;

    // This is as fast the object is allowed to go.
    public float speedLimit = 10.0f;

    // This is the speed that tells the function "Slow()" when to stop moving the object.
    public float minSpeed = 1.0f;

    // This is how long to pause inside "Slow()" before activating the function
    // "Accelerate()" to start the script again.
    public float stopTime = 1.0f;

    // This variable "currentSpeed" is the major player for dealing with velocity.
    // The "currentSpeed" is multiplied by the variable "acceleration" to speed up inside the function "Accelerate()".
    // Again, The "currentSpeed" is multiplied by the variable "inertia" to slow
    // things down inside the function "Slow()".
    private float currentSpeed = 0.0f;

    // The variable "functionState" controls which function, "Accelerate()" or "Slow()",
    // is active. "0" is function "Accelerate()" and "1" is function "Slow()".
    private float functionState = 0;

    // The next two variables are used to make sure that while the function "Accelerate()" is running,
    // the function "Slow()" can not run (as well as the reverse).
    private bool accelerationState;
    private bool slowState;

    // This variable will store the "active" target object (the waypoint to move to).
    private Transform waypoint;

    // This variable is an array. []< that is an array container if you didnt know.
    // It holds all the Waypoint Objects that you assign in the inspector.
    public Transform[] waypoints;

    // This variable keeps track of which Waypoint Object,
    // in the previously mentioned array variable "waypoints", is currently active.
    private int WPindexPointer;

    /// <summary>
    /// ///////////////////////////////////////////
    /// </summary>

    void Start() {
        controller = GetComponent<Controller2D> ();
        anim = GetComponent<Animator>();
		attackBool = false;
        functionState = 0;
    }

    void Update() {
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        //collisions.enemyHitBool is set when the controller raycast hits a collider with the enemy tag
        //Turn off the attack animation boolean on the next update
		if(attackBool){
			anim.SetBool("Attack", false);
		} else if(!attackBool && controller.enemyHitBool) {
			anim.SetBool("Attack", controller.enemyHitBool);
		}
		
        //Keyboard movement for testing
//        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
//        velocity.x = input.x * moveSpeed;
//        velocity.y += gravity * Time.deltaTime;
//        controller.Move (velocity * Time.deltaTime);

        /////////////////////////////////////////////////////////////////////
        //New movement
        // If functionState variable is currently "0" then run "Accelerate()".
        // Without the "if", "Accelerate()" would run every frame.
        if (functionState == 0)
        {
            Accelerate();
        }

        // If functionState variable is currently "1" then run "Slow()".
        // Without the "if", "Slow()" would run every frame.
        if (functionState == 1)
        {
            StartCoroutine(Slow());
        }

        waypoint = waypoints[WPindexPointer]; //Keep the object pointed toward the current Waypoint object.

        /////////////////////////////////////////////////////////////////////

        //This is mine
        anim.SetFloat("Speed", velocity.x);
    }

    public void Die(){
        Debug.Log("The Player is dead");
        gameObject.SetActive(false);
    }

    // I declared "Accelerate()".
    void Accelerate ()
    {
        if (accelerationState == false)
        {
            // Make sure that if Accelerate() is running, Slow() can not run.
            accelerationState = true;
            slowState = false;
        }

        // Now do the acceleration toward the active waypoint until the "speedLimit" is reached
        currentSpeed = currentSpeed + acceleration * acceleration;
        velocity.x = currentSpeed * direction;
        controller.Move (velocity * Time.deltaTime);
        //transform.Translate (0,0,Time.deltaTime * currentSpeed);

        // When the "speedlimit" is reached or exceeded ...
        if (currentSpeed >= speedLimit)
        {
            // ... turn off acceleration and set "currentSpeed" to be
            // exactly the "speedLimit". Without this, the "currentSpeed
            // will be slightly above "speedLimit"
            currentSpeed = speedLimit;
        }
    }

    //The function "OnTriggerEnter" is called when a collision happens.
    void OnTriggerEnter ()
    {
        // When the GameObject collides with the waypoint's collider,
        // activate "Slow()" by setting "functionState" to "1".
        functionState = 1;

        // When the GameObject collides with the waypoint's collider,
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

    // I declared "Slow()".
    IEnumerator Slow()
    {
        if (slowState == false) //
        {
            // Make sure that if Slow() is running, Accelerate() can not run.
            accelerationState = false;
            slowState = true;
        }

        // Begin to do the slow down (or speed up if inertia is set above "1.0" in the inspector).
        currentSpeed = currentSpeed * inertia;
		velocity.x = currentSpeed * direction;
		controller.Move (velocity * Time.deltaTime);
        //transform.Translate (0,0,Time.deltaTime * currentSpeed);

        // When the "minSpeed" is reached or exceeded ...
        if (currentSpeed <= minSpeed)
        {
            // ... Stop the movement by setting "currentSpeed to Zero.
            currentSpeed = 0.0f;
            // Wait for the amount of time set in "stopTime" before moving to next waypoint.
            yield return new WaitForSeconds(stopTime);
            // Activate the function "Accelerate()" to move to next waypoint.
            functionState = 0;
        }
    }
}
