
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    //Settings
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;

    private bool can_move;
    private float jump_force;
    private Vector2 velocity;

    private float spd;
    private float jump;
	private bool canJump;
    private float gravity;

    private float throw_power;
    private bool dead;

    private GameObject playercollider;
    private DoorBehavior door;
    private WeaponBehavior weapon;

	//Init Event
	void Awake () {
        //General
		gameObject.tag = "Player";
        gameObject.AddComponent<PixelSnap>();

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //Settings
        spd = 6f;
        jump = 3500f;
        gravity = 150f;

        //Variables
        can_move = true;
        jump_force = 0f;
        velocity = Vector2.zero;

        throw_power = 0;
        dead = false;

        door = null;
        weapon = null;

        //Player Collider
        GameObject player_collider = new GameObject("PlayerCollider");
        player_collider.transform.position = transform.position;
        player_collider.transform.parent = transform;
        player_collider.tag = "Human";
        player_collider.layer = 11;
        BoxCollider2D pl_col = player_collider.AddComponent<BoxCollider2D>();
        pl_col.size = GetComponent<BoxCollider2D>().size;
        pl_col.offset = GetComponent<BoxCollider2D>().offset;
        pl_col.isTrigger = true;
        Rigidbody2D pl_rb = player_collider.AddComponent<Rigidbody2D>();
        pl_rb.gravityScale = 0;
        pl_rb.freezeRotation = true;
        playercollider = player_collider;
	}
	
	//Update Event
	void Update () {
		bool[] controls = getControls();

        //Dead
        if (dead){
            velocity.x = 0f;
            anim.Play("player_dead");
            return;
        }

        //Movement
        velocity.x = 0f;
        if (can_move){
            //Jump
            if (controls[4]){
				if (door != null){
                    transform.position = door.findDoor();
                    if (weapon != null){
                        weapon.transform.position = door.findDoor();
                        weapon.pickup();
                    }
                    door = null;
                }
                else {
					if (canJump) {
						jump_force = jump;
						canJump = false;
					}

                }
            }

            //Move Left and Right
            float move_spd = spd;
            float slow_mult = 0.9f;
            if (getCursorPosition().x < transform.position.x){
                sr.flipX = true;
            }
            else {
                sr.flipX = false;
            }

            if (controls[1]){
                //Left
                if (!sr.flipX){
                    move_spd *= slow_mult;
                }
                velocity.x = -move_spd;
            }
            else if (controls[3]){
                //Right
                if (sr.flipX){
                    move_spd *= slow_mult;
                }
                velocity.x = move_spd;
            }
        }

        //Weapons
        if (weapon != null){
            //Weapon Scale
            weapon.GetComponent<SpriteRenderer>().flipX = sr.flipX;

            //Weapon Angle
            float angle = Mathf.Atan2(getCursorPosition().y - transform.position.y, getCursorPosition().x - transform.position.x) * Mathf.Rad2Deg;
            float use_angle = Mathf.Clamp(angle, -45f, 45f);
            if (sr.flipX){
                if (angle > 90){
                    use_angle = Mathf.Clamp(angle, 135f, 180f);
                }
                else if (angle < -80){
                    use_angle = Mathf.Clamp(angle, -180f, -135f);
                }

                Vector2 flip_cursor_pos = new Vector2(Mathf.Abs(getCursorPosition().x - transform.position.x) + transform.position.x, transform.position.y - (getCursorPosition().y - transform.position.y));
                angle = Mathf.Atan2(flip_cursor_pos.y - transform.position.y, flip_cursor_pos.x - transform.position.x) * Mathf.Rad2Deg;
            }
            angle = Mathf.Clamp(angle, -45f, 45f);
            weapon.transform.eulerAngles = new Vector3(0f, 0f, angle);
            weapon.setUseAngle(use_angle * Mathf.Deg2Rad);

            //Buttons
            bool throw_weapon = true;
            bool[] cursor_buttons = getCursorButtons();
            if (cursor_buttons[2]){
                if (weapon.canuse){
                    weapon.useWeapon();
                }
            }
            else if (cursor_buttons[3]){
                throw_weapon = false;
                if (throw_power == 0){
                    throw_power = 0.35f;
                }
                throw_power += 0.035f;
                throw_power = Mathf.Clamp(throw_power, 0, 1);
            }

            if (throw_weapon){
                if (throw_power > 0){
                    if (weapon != null){
                        weapon.throwWeapon(use_angle * Mathf.Deg2Rad, throw_power);

                        weapon = null;
                        throw_power = 0;
                    }
                }
            }
        }

        //Animation
        if (!canJump){
            anim.Play("player_jump");
        }
        else {
            if (velocity.x != 0){
                if (Mathf.Abs(velocity.x) < spd){
                    anim.Play("player_slowwalk");
                }
                else {
                    anim.Play("player_walk");
                }
            }
            else {
                anim.Play("player_idle");
            }
        }

        if (dead){
            anim.Play("player_dead");
        }
	}

    //Fixed Update
    void FixedUpdate () {
        //Physics
        if (jump_force > 0){
            velocity.y = jump_force;
            jump_force = 0;
        }
        else {
            velocity.y -= gravity;
            if (velocity.y < 0){
                velocity.y *= 1.15f;
            }
            else {
                velocity.y *= 0.85f;
            }
            velocity.y = Mathf.Clamp(velocity.y, -4000f, velocity.y);
        }
        rb.velocity = new Vector2(velocity.x, 0f);
        rb.AddForce(new Vector2(0, velocity.y));

        //Player Collider
        if (playercollider != null){
            playercollider.transform.position = transform.position;
        }

        //Weapon Position
        if (weapon != null){
            float weapon_offset_x = 0.15f;
            float weapon_offset_y = 0.05f;
            if (sr.flipX){
                weapon_offset_x = -weapon_offset_x;
            }

            weapon.setTargetPosition(new Vector2(transform.position.x + weapon_offset_x, transform.position.y + weapon_offset_y));
        }
    }

    //Collisions
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Door"){
            door = collision.gameObject.GetComponent<DoorBehavior>();
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Door"){
            door = null;
        }
    }

	void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Floor")
		{
			canJump = true;
		}
	}

    void OnCollisionExit2D(Collision2D collision)
	{
		if(collision.gameObject.tag == "Floor")
		{
			canJump = false;
		}
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
    /// The array has two entires: Interact, Throw, Hold Interact, Hold Throw
    /// </summary>
    public bool[] getCursorButtons() {
        bool[] controls = new bool[4];
        if (Input.GetMouseButtonDown(0)){
            controls[0] = true;
        }
        if (Input.GetMouseButtonDown(1)){
            controls[1] = true;
        }
        if (Input.GetMouseButton(0)){
            controls[2] = true;
        }
        if (Input.GetMouseButton(1)){
            controls[3] = true;
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

    public void kill() {
        dead = true;
        can_move = false;
        if (weapon != null){
            weapon.dropWeapon();
        }
    }

    public void setWeapon(WeaponBehavior new_weapon){
        weapon = new_weapon;
    }

    /// <summary>
    /// canmove: returns if the player can move
    /// </summary>
    public bool canmove {
        get {
            return can_move;
        }
    }

    /// <summary>
    /// level: returns the Level number the player is on
    /// </summary>
    public int level {
        get {
            return LevelManager.instance.getLevelNum(transform.position.y);
        }
    }

    public bool hasWeapon {
        get {
            if (weapon != null){
                return true;
            }
            return false;
        }
    }

}
