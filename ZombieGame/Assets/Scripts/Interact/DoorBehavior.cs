﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : InteractBehavior {

	//Settings
    [SerializeField] private int id;

    private int levelnum;

    //Init & Update Events
	protected override void init () {
        //Settings
        gameObject.tag = "Door";
        col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        levelnum = LevelManager.instance.getLevelNum(transform.position.y);

        base.init();
	}
	
	protected override void step () {
        //Put Door behavior here

		base.step();
	}

    //Public Methods
    public Vector2 findDoor() {
        GameObject[] search = GameObject.FindGameObjectsWithTag("Door");
        Vector2 return_v2 = Vector2.zero;
        for (int i = 0; i < search.Length; i++){
            if (search[i] != gameObject){
                if (search[i].GetComponent<DoorBehavior>().door_id == id){
                    return_v2 = search[i].transform.position;
                    break;
                }
            }
        }
        return return_v2;
    }

    public int door_id {
        get {
            return id;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;

        Vector3 door_pos = new Vector3(findDoor().x, findDoor().y, transform.position.z);
        Gizmos.DrawSphere(door_pos, 0.15f);
    }
}