using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : InteractBehavior {

	//Settings
    private bool zombie;
    private string behavior;

    private float aware_r = 3;
    private float spd;
    private int level;

    //Init
    protected override void init() {
        zombie = true;
        behavior = "normal";

        if (zombie){
            gameObject.tag = "Zombie";
        }
        else {
            gameObject.tag = "Human";
        }

        level = LevelManager.instance.getLevelNum(transform.position.y);
    }

    //Update
    protected override void step() {
        if (zombie == true){
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            List<GameObject> humans = new List<GameObject>(GameObject.FindGameObjectsWithTag("Human"));
            humans.Add(player);

            if (behavior == "normal"){
                bool near = false;
                Vector2 near_pos = Vector2.zero;

                for (int i = 0; i < humans.Count; i++){
                    if (Vector2.Distance(humans[i].transform.position, transform.position) <= aware_r){
                        near = true;
                        near_pos = humans[i].transform.position;
                        break;
                    }
                }
            }
        }
        else {

        }

        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector2 draw_pos = new Vector2(transform.position.x + (Mathf.Cos(0) * aware_r), transform.position.y + (Mathf.Sin(0) * aware_r));
        for (int i = 15; i < 360; i += 15){
            float draw_rad = i * Mathf.Deg2Rad;
            Vector2 new_pos = new Vector2(transform.position.x + (Mathf.Cos(draw_rad) * aware_r), transform.position.y + (Mathf.Sin(draw_rad) * aware_r));
            Gizmos.DrawLine(new Vector3(draw_pos.x, draw_pos.y, transform.position.z), new Vector3(new_pos.x, new_pos.y, transform.position.z));
            draw_pos = new_pos;
        }
    }

}
