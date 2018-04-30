using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour {

	private GameObject player;
	private int count = -1;
	[SerializeField] private float reduction = .1f;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(count > 0)
		{
			Color temp = transform.GetChild (count).GetComponent<SpriteRenderer> ().color;
			temp.a -= reduction;
			transform.GetChild (count).GetComponent<SpriteRenderer> ().color = temp;
		}
		count = LevelManager.instance.getLevelNum (player.transform.position.y);
		if (Time.timeSinceLevelLoad < 1f) 
		{
			count = 0;
		}
	}
}
