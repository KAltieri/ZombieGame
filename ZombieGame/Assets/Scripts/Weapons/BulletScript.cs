using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    //Settings
    private Vector2 size;
    private bool delay;
    private Vector2 previous_pos;
    private float damage_val;

    void Start() {
        size = GetComponent<BoxCollider2D>().size;
        delay = true;
        previous_pos = transform.position;
    }

    void Update () {
        GameObject[] NPCs = GameObject.FindGameObjectsWithTag("Zombie");
        if (NPCs.Length > 0){
            GameObject closest = NPCs[0];
            float distance = Vector2.Distance(NPCs[0].transform.position, transform.position);
            for (int i = 0; i < NPCs.Length; i++){
                float new_dis = Vector2.Distance(NPCs[i].transform.position, transform.position);
                if (new_dis < distance){
                    closest = NPCs[i];
                    distance = new_dis;
                }
            }

            if (distance < 0.45f){
                transform.position = new Vector2(closest.transform.position.x, Mathf.Lerp(transform.position.y, closest.transform.position.y, 0.1f));
            }
        }

		if (Vector2.Distance(transform.position, Camera.main.transform.position) > 16f){
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Zombie"){
            if (Vector2.Distance(transform.position, new Vector2(collision.transform.position.x, collision.transform.position.y + 0.25f)) < 0.4f){
                collision.gameObject.GetComponent<AIScript>().damage(5);
            }
            else {
                collision.gameObject.GetComponent<AIScript>().damage(damage_val);
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Human"){
            collision.gameObject.GetComponent<AIScript>().damage(5);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Floor"){
            Destroy(gameObject);
        }
    }

    public float damage {
        set {
            damage_val = value;
        }
    }

}
