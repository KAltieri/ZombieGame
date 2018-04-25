
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    //Settings
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;

    private bool can_move;
    private float gravity;
    private float jump;
    private float jump_force;
    private Vector2 velocity;

    private float spd;

	//Init Event
	void Awake () {
        //General
		gameObject.tag = "Player";
        gameObject.AddComponent<PixelSnap>();
        
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        //Settings
        spd = 10f;
        jump = 10f;
        gravity = 1f;

        //Variables
        can_move = true;
        jump_force = 0f;
        velocity = Vector2.zero;
	}
	
	//Update Event
	void Update () {
		bool[] controls = getControls();

        //Movement
        velocity.x = 0f;
        velocity.y = rb.velocity.y;
        if (can_move){
            //Jump
            if (controls[4]){
                jump_force = jump;
            }

            //Move Left and Right
            if (controls[1]){
                //Left
                velocity.x = -spd;
            }
            else if (controls[3]){
                //Right
                velocity.x = spd;
            }
        }
	}

    //Fixed Update
    void FixedUpdate () {
        if (jump_force > 0){
            velocity.y = 0;
        }
        else {
            velocity.y -= gravity;
        }
        rb.velocity = new Vector2(velocity.x, velocity.y + jump_force);
        jump_force = 0;
    }

    //Public Methods
    /// <summary>
    /// getCursorPosition(): returns a Vector2 of the cursor in the game
    /// </summary>
    public Vector2 getCursorPosition() {
        Vector3 v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(v3.x, v3.y);
    }

    /// <summary>
    /// getCursorButtons(): returns the buttons of the cursor in the game as a bool array
    /// The array has two entires: Interact, Throw
    /// </summary>
    public bool[] getCursorButtons() {
        bool[] controls = new bool[2];
        if (Input.GetMouseButtonDown(0)){
            controls[0] = true;
        }
        if (Input.GetMouseButtonDown(1)){
            controls[1] = true;
        }
        return controls;
    }

    /// <summary>
    /// getControls(): returns the 4 directions the player and if the player pressed jump as a bool array
    /// The array has 5 entries: Up, Left, Down, Right, and Jump
    /// </summary>
    public bool[] getControls() {
        bool[] controls = new bool[5];
        if (Input.GetKey(KeyCode.W)){
            controls[0] = true;
        }
        if (Input.GetKey(KeyCode.A)){
            controls[1] = true;
        }
        if (Input.GetKey(KeyCode.S)){
            controls[2] = true;
        }
        if (Input.GetKey(KeyCode.D)){
            controls[3] = true;
        }
        if (Input.GetKeyDown(KeyCode.W)){
            controls[4] = true;
        }
        return controls;
    }

    /// <summary>
    /// level: returns the Level number the player is on
    /// </summary>
    public int level {
        get {
            return LevelManager.instance.getLevelNum(transform.position.y);
        }
    }
}
