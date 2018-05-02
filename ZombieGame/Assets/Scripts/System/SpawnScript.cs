using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour {

	GameObject player;
	GameObject[] zSpawn;
	Transform spawnPosR;
	Transform spawnPosL;

	[Header("Level Difficulty")]
	[SerializeField] float timeWait = 30;
	[SerializeField] float timeIncrease = 10;
	[SerializeField] float speedIncrease;
	float tempTime = 0;
	float tempTime2 = 5;
	int count = 0;
	int tempCount = 0;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		zSpawn = GameObject.FindGameObjectsWithTag ("Spawn");
		if (zSpawn [0].transform.position.x > zSpawn [1].transform.position.x) {
			spawnPosR = zSpawn [0].GetComponent<Transform> ();
			spawnPosL = zSpawn [1].GetComponent<Transform> ();
		} 
		else {
			spawnPosR = zSpawn [1].GetComponent<Transform> ();
			spawnPosL = zSpawn [0].GetComponent<Transform> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (tempTime < 0) {
            spawn(spawnPosR.position);
			tempTime = timeWait + Random.Range(-0.5f, 0.5f);
		}
		if (tempTime2 < 0) {
            spawn(spawnPosL.position);
            tempTime2 = timeWait + Random.Range(-0.5f, 0.5f);
		}
		tempTime -= Time.deltaTime;
		tempTime2 -= Time.deltaTime;
		tempCount = LevelManager.instance.getLevelNum(player.transform.position.y);
		if (tempCount > count) {
			increaseDifficulty ();
		}
        GameObject[] NPCs_pos = GameObject.FindGameObjectsWithTag("Human");
        for (int i = 0; i < NPCs_pos.Length; i++){
            if (LevelManager.instance.getLevelNum(NPCs_pos[i].transform.position.y) > count) {
                count = LevelManager.instance.getLevelNum(NPCs_pos[i].transform.position.y);
            }
        }
	}

	void increaseDifficulty()
	{
		//zombie speed increase will go here
		timeWait -= timeIncrease;
		count = tempCount;
	}

    private void spawn(Vector3 spawn_pos) {
        if (player.GetComponent<PlayerBehavior>().canmove){
			GameObject zom_obj = Instantiate (Resources.Load ("pZombie") as GameObject, spawn_pos, transform.rotation);
            AIScript zom = zom_obj.GetComponent<AIScript>();
            if (count > 12){
                zom.setSpeed(0.08f);
            }
            else if (count > 5){
                int chance = Random.Range(0, 20);
                if (chance < 2) {
                    zom.setSpeed(0.065f);
                }
                else if (chance < 5) {
                    zom.setSpeed(0.05f);
                }
                else if (chance < 15) {
                    zom.setSpeed(0.04f);
                }
                else {
                    zom.setSpeed(0.005f + (speedIncrease * tempCount) + Random.Range(0.005f, 0.015f));
                }
            }
            else if (count > 0){
                int chance = Random.Range(0, 20);
                if (chance < 5) {
                    zom.setSpeed(0.043f);
                }
                else if (chance < 10) {
                    zom.setSpeed(0.025f);
                }
                else {
                    zom.setSpeed(0.005f + (speedIncrease * tempCount) + Random.Range(0.005f, 0.015f));
                }
            }
        }
    }

}
