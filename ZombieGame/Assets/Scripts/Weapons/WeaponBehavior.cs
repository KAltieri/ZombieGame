using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WeaponBehavior : InteractBehavior {

    //Settings
    protected bool active;
    protected bool thrown;
    protected bool player;
    protected Rigidbody2D rb;
    [SerializeField] protected string weapon_name = "Weapon_Name";
    [SerializeField] protected bool meele = false;
    [SerializeField] protected float meele_damage = 0.25f;

    [SerializeField] protected float spd = 5f;
    [SerializeField] protected float friction = 0.15f;
    [SerializeField] protected float recoil = 0.5f;
    [SerializeField] protected int cooldown = 20;
    [SerializeField] protected int ammo = -1;
    
    protected float use_angle;
    protected int timer;

    protected Vector2 velocity;
    protected Vector2 active_position; 
    protected Vector2 target_position;
    protected Collider2D ignore_collider;

    private Collider2D actual_collider;
    private Collider2D click_collider;

    private void Awake() {
        gameObject.tag = "Weapon";
        gameObject.layer = 10;

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0;

        if (GetComponent<Collider2D>() == null){
            actual_collider = gameObject.AddComponent<PolygonCollider2D>();
        }
        else {
            actual_collider = GetComponent<Collider2D>();
        }
        click_collider = gameObject.AddComponent<BoxCollider2D>();
        click_collider.isTrigger = true;

        velocity = Vector2.zero;
    }

    //Init & Update Events
    protected override void init(){
        base.init();
        active = false;
        thrown = false;
        player = false;

        active_position = transform.position;
        timer = 0;
        use_angle = 0;
        col = click_collider;
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
    }

    protected override void step(){
        base.step();
        if (active){
            //Outline
            sprout.OutlineColor = new Color(1, 1, 1, 0);

            //Cooldown
            if (timer > 0){
                timer--;
            }

            //Position
            active_position = new Vector2(Mathf.Lerp(active_position.x, target_position.x, spd) + velocity.x, Mathf.Lerp(active_position.y, target_position.y, spd) + velocity.y);
            transform.position = new Vector3(active_position.x - (active_position.x % 0.03125f), active_position.y - (active_position.y % 0.03125f), transform.position.z);
            
            //Velocity & Recoil
            velocity = new Vector2(Mathf.Lerp(velocity.x, 0, friction), Mathf.Lerp(velocity.y, 0, friction));
        }
        else {
            PlayerBehavior player_obj = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
            if (Vector2.Distance(player_obj.transform.position, transform.position) < 2.5f){
                if (!player_obj.hasWeapon){
                    if (selected){
                        if (player_obj.getCursorButtons()[0]){
                            pickup();
                            player = true;
                            player_obj.setWeapon(this);
                            timer = 15;
                            ignore_collider = player_obj.GetComponent<Collider2D>();
                            Physics2D.IgnoreCollision(actual_collider, ignore_collider, true);
                        }
                    }
                }
                else {
                    sprout.OutlineColor = new Color(0f, 0f, 0f, 0f);
                }
            }
            else {
                sprout.OutlineColor = new Color(0f, 0f, 0f, 0f);
            }

            if (thrown){
                if (Mathf.Max(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y)) < 0.05f){
                    thrown = false;
                    gameObject.layer = 10;
                    Physics2D.IgnoreCollision(actual_collider, ignore_collider, false);
                }
            }
        }
    }

    protected override void use(){
        //Check Cooldown & Ammo
        if (timer > 0){
            return;
        }
        
        if (ammo == 0){
            return;
        }
        else if (ammo > 0){
            ammo--;
        }
        timer = cooldown;

        //Use Weapon
        velocity = new Vector2(Mathf.Cos(use_angle) * recoil, Mathf.Sin(use_angle) * recoil);

        //Create HitBox
        if (meele){
            if (transform.childCount > 0){
                for (int i = transform.childCount - 1; i >= 0; i--){
                    Destroy(transform.GetChild(i).gameObject);
                }
            }

            GameObject hitbox = new GameObject("hitbox");

            PolygonCollider2D this_poly_col = (PolygonCollider2D) actual_collider;
            PolygonCollider2D poly_col = hitbox.AddComponent<PolygonCollider2D>();
            poly_col.points = this_poly_col.points;
            poly_col.offset = this_poly_col.offset;

            Physics2D.IgnoreCollision(hitbox.GetComponent<Collider2D>(), ignore_collider, true);
            hitbox.layer = 11;
            hitbox.transform.position = transform.position;
            hitbox.transform.eulerAngles = transform.eulerAngles;
            hitbox.transform.parent = transform;
            hitbox.AddComponent<MeeleHitBox>().damage = meele_damage;
            hitbox.GetComponent<MeeleHitBox>().velocity = velocity;
        }
    }

    //Collision
    void OnCollisionEnter2D(Collision2D collision) {
        if (thrown){
            if (collision.gameObject.tag == "Zombie"){
                thrown = false;
                gameObject.layer = 10;
                Physics2D.IgnoreCollision(actual_collider, ignore_collider, false);
                collision.gameObject.GetComponent<AIScript>().damage(meele_damage * 2);
                if (meele){
                    collision.gameObject.GetComponent<AIScript>().damage(5);
                    Destroy(gameObject);
                }
            }
        }
    }
    
    //Public Methods
    public void pickup(){
        active = true;
        active_position = transform.position;
        target_position = transform.position;

        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
        rb.gravityScale = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
    }

    public void setTargetPosition(Vector2 position){
        target_position = position;
    }

    public void setUseAngle(float angle){
        use_angle = angle;
    }

    public void useWeapon() {
        use();
    }

    public void throwWeapon(float angle, float power) {
        active = false;
        thrown = true;
        if (player){
            Camera.main.GetComponent<CameraBehavior>().bulletUI.hideUI();
        }
        player = false;
        gameObject.layer = 11;

        rb.freezeRotation = false;
        rb.gravityScale = 1;

        power = Mathf.Clamp(power, 0f, 1f) * 0.7f;
        power = power * 550;
        rb.AddForce(new Vector2(Mathf.Cos(angle) * power, Mathf.Sin(angle) * power));
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
    }

    public void dropWeapon(){
        active = false;
        if (player){
            Camera.main.GetComponent<CameraBehavior>().bulletUI.hideUI();
            gameObject.layer = 12;
        }
        player = false;

        rb.AddForce(new Vector2(UnityEngine.Random.Range(1f, -1f) * 100, 200));
        rb.freezeRotation = false;
        rb.gravityScale = 1;
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
    }

    public bool canuse {
        get {
            if (ammo == 0){
                return false;
            }
            if (timer > 0){
                return false;
            }
            return true;
        }
    }

}

public class MeeleHitBox : MonoBehaviour {

    //Settings
    private float timer;
    private float damage_val;
    private Vector2 kvelocity;

    void Start() {
        timer = 8;
    }

    void Update() {
        timer--;
        if (timer <= 0){
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Zombie"){
            float knockback = 0.05f;
            if (kvelocity.x < 0){
                collision.transform.position = new Vector3(collision.transform.position.x - knockback, collision.transform.position.y, collision.transform.position.z);
            }
            else if (kvelocity.x > 0){
                collision.transform.position = new Vector3(collision.transform.position.x + knockback, collision.transform.position.y, collision.transform.position.z);
            }

            collision.gameObject.GetComponent<AIScript>().damage(damage_val);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Human"){
            collision.gameObject.GetComponent<AIScript>().damage(5);
            Destroy(gameObject);
        }
    }

    public float damage {
        set {
            damage_val = value;
        }
    }

    public Vector2 velocity {
        set {
            kvelocity = value;
        }
    }

}