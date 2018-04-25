using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : InteractBehavior {

	//Settings
    private bool zombie;
    private string behavior;

    private float aware_r = 3;
    private float spd = 0.01f;
    private int level;

    private bool door_found;
    private bool door_move;
    private DoorBehavior door;

    //Init
    protected override void init() {
        base.init();

        zombie = true;
        behavior = "normal";

        if (zombie){
            gameObject.tag = "Zombie";
        }
        else {
            gameObject.tag = "Human";
        }

        door_found = false;
        door_move = false;
        door = null;
        level = LevelManager.instance.getLevelNum(transform.position.y);
    }

    //Update
    protected override void step() {
        base.step();

        //Movement
        Vector2 velocity = Vector2.zero;
        if (zombie == true){
            //Check for Player or Humans
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            List<GameObject> humans = new List<GameObject>(GameObject.FindGameObjectsWithTag("Human"));
            humans.Add(player);

            bool near = false;
            Vector2 near_pos = Vector2.zero;
            Vector2 target_pos = Vector2.zero;

            for (int i = 0; i < humans.Count; i++){
                if (Vector2.Distance(humans[i].transform.position, transform.position) <= aware_r){
                    near = true;
                    near_pos = humans[i].transform.position;
                    break;
                }
            }

            //Move towards Door or Humans
            if (near){
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

            if (transform.position.x < target_pos.x){
                velocity.x = spd;
            }
            else if (transform.position.x > target_pos.x){
                velocity.x = -spd;
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

        }

        transform.position = new Vector2(transform.position.x + velocity.x, LevelManager.instance.getLevelY(level));
    }

    //Collisions
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Door"){
            if (collision.gameObject.GetComponent<DoorBehavior>() == door){
                door_move = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Door"){
            if (collision.gameObject.GetComponent<DoorBehavior>() == door){
                door_move = false;
            }
        }
    }

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
