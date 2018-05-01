using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : InteractBehavior {

	//Settings
    private Animator anim;

    [SerializeField] private bool zombie;
    [SerializeField] private string behavior;
    private bool canmove;
    private CorpseBehavior eat;
    private bool dead;

    private float health;
    private float aware_r = 3;
    [SerializeField] private float spd = 0.005f;
    private int level;

    private Color human_color;
    private int human_timer;
    private AIScript human_bite;

    private bool door_found;
    private bool door_move;
    private DoorBehavior door;

    //Init
    private void Awake() {
        if (zombie){
            gameObject.tag = "Zombie";
            if (spd == 0.005f){
                spd = Random.Range(0.005f, 0.01f);
            }
        }
        else {
            gameObject.tag = "Human";

            human_timer = 160;
            BoxCollider2D box_col = GetComponent<BoxCollider2D>();
            if (box_col == null){
                box_col = gameObject.AddComponent<BoxCollider2D>();
                col = box_col;
            }
            GameObject human_col_obj = new GameObject("Human_Collider");
            human_col_obj.layer = 11;
            human_col_obj.tag = "Human";
            human_col_obj.transform.position = transform.position;
            human_col_obj.transform.parent = transform;
            BoxCollider2D human_collider = human_col_obj.AddComponent<BoxCollider2D>();
            human_collider.size = box_col.size;
            human_collider.offset = box_col.offset;
            human_collider.isTrigger = true;
            Rigidbody2D human_rb = human_col_obj.AddComponent<Rigidbody2D>();
            human_rb.gravityScale = 0;
            human_rb.freezeRotation = true;
        }
    }

    protected override void init() {
        base.init();
        
        anim = GetComponent<Animator>();

        behavior = "normal";
        canmove = true;
        eat = null;
        dead = false;

        health = 1;
        door_found = false;
        door_move = false;
        door = null;
        level = LevelManager.instance.getLevelNum(transform.position.y);

        human_color = new Color(Random.Range(30f, 80f) / 255f, Random.Range(180f, 90f) / 255f, 200f / 255f, 1f);
        human_bite = null;
    }

    //Update
    protected override void step() {
        base.step();

        //Dead
        if (dead){
            sprout.OutlineColor = new Color(0f, 0f, 0f, 0f);
            if (zombie){
                anim.Play("zombie_dead");
            }
            else {
                anim.Play("human_dead");
            }

            return;
        }

        //Zombify
        if (zombie){
            if (human_bite != null){
                anim.Play("zombie_idle");
                if (human_bite.isZombie){
                    human_bite = null;
                }
                return;
            }
        }
        else {
            if (human_timer < 160){
                if (human_timer > 0){
                    human_timer--;

                    anim.Play("human_idle");
                    sr.color = Color.Lerp(human_color, new Color(75f / 255f, 105f / 255f, 47f / 255f, 1f), (160 - human_timer) / 160f);
                    if (human_timer == 0){
                        zombie = true;
                        gameObject.tag = "Zombie";
                    }
                    return;
                }
            }
        }

        //Eating
        if (eat != null){
            eat.eat();

            anim.Play("zombie_eat");
            return;
        }

        //Movement
        Vector2 velocity = Vector2.zero;
        if (canmove){
            //Variables
            Vector2 near_pos = Vector2.zero;
            Vector2 target_pos = Vector2.zero;
            if (zombie == true){
                //Check for Corpses
                GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
                bool found_corpse = false;

                float corpse_dis = 0;
                Vector2 corpse_pos = Vector2.zero;
                if (corpses.Length > 0){
                    corpse_dis = Vector2.Distance(transform.position, corpses[0].transform.position);
                    corpse_pos = corpses[0].transform.position;
                    for (int c = 0; c < corpses.Length; c++){
                        if (Vector2.Distance(corpses[c].transform.position, transform.position) <= aware_r){
                            if (LevelManager.instance.getLevelNum(corpses[c].transform.position.y) == level) {
                                found_corpse = true;

                                float new_corpse_dis = Vector2.Distance(corpses[c].transform.position, transform.position);
                                if (new_corpse_dis < corpse_dis){
                                    corpse_dis = new_corpse_dis;
                                    corpse_pos = corpses[c].transform.position;
                                }
                            }
                        }
                    }
                }

                //Check for Player or Humans
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                List<GameObject> humans = new List<GameObject>(GameObject.FindGameObjectsWithTag("Human"));
                humans.Add(player);

                bool near = false;
                for (int i = 0; i < humans.Count; i++){
                    if (Vector2.Distance(humans[i].transform.position, transform.position) <= aware_r){
                        if (LevelManager.instance.getLevelNum(humans[i].transform.position.y) == level) {
                            near = true;
                            near_pos = humans[i].transform.position;
                            break;
                        }
                    }
                }

                if (Vector2.Distance(player.transform.position, transform.position) < 0.2f){
                    anim.Play("zombie_eat");
                    return;
                }

                //Move towards Door or Humans or Corpses
                if (found_corpse){
                    target_pos = corpse_pos;
                }
                else if (near){
                    target_pos = near_pos;
                }
                else {
                    if (!door_found){
                        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
                        List<DoorBehavior> up_doors = new List<DoorBehavior>();

                        for (int i = 0; i < doors.Length; i++){
                            if (level == LevelManager.instance.getLevelNum(doors[i].transform.position.y)){
                                if (doors[i].GetComponent<DoorBehavior>().up){
                                    up_doors.Add(doors[i].GetComponent<DoorBehavior>());
                                }
                            }
                        }

                        if (up_doors.Count > 0){
                            door_found = true;
                            door = up_doors[Random.Range(0, up_doors.Count)];
                        }
                        else {
                            target_pos = player.transform.position;
                        }
                    }
                    else {
                        target_pos = new Vector2(door.transform.position.x, door.transform.position.y);
                    }
                }

                //Move Up Doors
                if (door_move){
                    transform.position = door.findDoor();
                    level = LevelManager.instance.getLevelNum(transform.position.y);

                    door_move = false;
                    door_found = false;
                    door = null;
                }
            }
            else {
                //Move towards Door or Humans or Corpses
                if (!door_found){
                    GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
                    List<DoorBehavior> up_doors = new List<DoorBehavior>();

                    for (int i = 0; i < doors.Length; i++){
                        if (level == LevelManager.instance.getLevelNum(doors[i].transform.position.y)){
                            if (doors[i].GetComponent<DoorBehavior>().up){
                                up_doors.Add(doors[i].GetComponent<DoorBehavior>());
                            }
                        }
                    }

                    if (up_doors.Count > 0){
                        door_found = true;
                        door = up_doors[Random.Range(0, up_doors.Count)];
                    }
                }
                else {
                    target_pos = new Vector2(door.transform.position.x, door.transform.position.y);
                }

                //Move Up Doors
                if (door_move){
                    transform.position = door.findDoor();
                    level = LevelManager.instance.getLevelNum(transform.position.y);

                    door_move = false;
                    door_found = false;
                    door = null;
                }
            }

            if (transform.position.x < target_pos.x){
                velocity.x = spd;
            }
            else if (transform.position.x > target_pos.x){
                velocity.x = -spd;
            }
        }

        transform.position = new Vector2(transform.position.x + velocity.x, LevelManager.instance.getLevelY(level));

        //Animation
        if (velocity.x != 0){
            if (velocity.x < 0){
                sr.flipX = true;
            }
            else {
                sr.flipX = false;
            }
        }

        if (zombie){
            sr.color = new Color(1, 1, 1, 1);
            if (velocity.x != 0){
                anim.Play("zombie_walk");
            }
            else {
                anim.Play("zombie_idle");
            }
        }
        else {
            sr.color = human_color;
            if (velocity.x != 0){
                anim.Play("human_walk");
            }
            else {
                anim.Play("human_idle");
            }
        }
    }

    //Collisions
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Door"){
            if (collision.gameObject.GetComponent<DoorBehavior>() == door){
                door_move = true;
            }
        }

        if (zombie){
            if (collision.gameObject.tag == "Corpse") {
                if (zombie){
                    if (eat == null){
                        eat = collision.gameObject.GetComponent<CorpseBehavior>();
                    }
                }
            }
            else if (collision.gameObject.tag == "Human"){
                if (eat == null){
                    if (human_bite == null){
                        if (collision.gameObject.transform.parent.gameObject.GetComponent<PlayerBehavior>() != null){
                            collision.gameObject.transform.parent.gameObject.GetComponent<PlayerBehavior>().kill();
                            return;
                        }
                        human_bite = collision.gameObject.transform.parent.gameObject.GetComponent<AIScript>();
                        human_bite.zombify();
                        Destroy(collision.gameObject);
                    }
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Door"){
            if (collision.gameObject.GetComponent<DoorBehavior>() == door){
                door_move = false;
            }
        }

        if (zombie){
            if (collision.gameObject.tag == "Corpse") {
                if (eat != null){
                    if (collision.gameObject.GetComponent<CorpseBehavior>() == eat){
                        eat = null;
                    }
                }
            }
        }
    }

    //Public Methods
    public void setSpeed(float new_spd){
        spd = new_spd;
    }

    public void damage(float dmg){
        health -= dmg;
        if (health <= 0){
            dead = true;
            canmove = false;
            gameObject.layer = 12;
            if (!zombie){
                GameObject corpse = Instantiate(Resources.Load("Weapons/pCorpse") as GameObject, new Vector3(transform.position.x, transform.position.y - 0.20f, transform.position.z), transform.rotation);
                corpse.GetComponent<SpriteRenderer>().color = human_color;
                if (transform.childCount > 0){
                    Destroy(transform.GetChild(0).gameObject);
                }
                Destroy(gameObject);
            }
        }
    }

    public void zombify() {
        human_timer--;
    }

    public bool isZombie {
        get {
            return zombie;
        }
    }

    //Debug
    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector2 draw_pos = new Vector2(transform.position.x + (Mathf.Cos(0) * aware_r), transform.position.y + (Mathf.Sin(0) * aware_r));
        for (int i = 15; i <= 360; i += 15){
            float draw_rad = i * Mathf.Deg2Rad;
            Vector2 new_pos = new Vector2(transform.position.x + (Mathf.Cos(draw_rad) * aware_r), transform.position.y + (Mathf.Sin(draw_rad) * aware_r));
            Gizmos.DrawLine(new Vector3(draw_pos.x, draw_pos.y, transform.position.z), new Vector3(new_pos.x, new_pos.y, transform.position.z));
            draw_pos = new_pos;
        }
    }

}
