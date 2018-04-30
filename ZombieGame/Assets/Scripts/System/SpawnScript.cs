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
			Instantiate (Resources.Load ("sZombie_Debug"), spawnPosR.position, spawnPosR.rotation);
			tempTime = timeWait;
		}
		if (tempTime2 < 0) {
			Instantiate (Resources.Load ("sZombie_Debug"), spawnPosL.position, spawnPosL.rotation);
			tempTime2 = timeWait;
		}
		tempTime -= Time.deltaTime;
		tempTime2 -= Time.deltaTime;
		tempCount = LevelManager.instance.getLevelNum (player.transform.position.y);
		if (tempCount > count) {
			Debug.Log (tempCount);
			increaseDifficulty ();
		}
	}

	void increaseDifficulty()
	{
		//zombie speed increase will go here
		timeWait -= timeIncrease;
		count = tempCount;
	}
}
