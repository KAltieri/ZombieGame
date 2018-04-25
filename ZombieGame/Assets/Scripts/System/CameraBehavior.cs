using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	//Settings
    [SerializeField] float spd = 0.05f;
    private Vector2 start_position;
    private Vector2 end_position;

    private float screenshaketime;
    private float screenshakeinten;
    private float screenshakedelay;
    private Vector2 screen_shake_pos;

    //Init Event
	void Start () {
		start_position = transform.position;
        start_position = end_position;

        screenshaketime = 0f;
	}
	
	//Update Event
	void FixedUpdate () {
        //Lerp Camera
        end_position = new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, LevelManager.instance.getLevelY(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>().level));
        start_position = new Vector2(Mathf.Lerp(start_position.x, end_position.x, Time.deltaTime * spd), Mathf.Lerp(start_position.y, end_position.y, Time.deltaTime * spd));
        transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        //Screen Shake
        if (screenshaketime > 0){
            screenshaketime -= Time.deltaTime;
            screenshakedelay -= Time.deltaTime;
            if (screenshakedelay <= 0){
                screenshakedelay = 0.1f;
                start_position = new Vector2(start_position.x + Random.Range(-screenshakeinten, screenshakeinten), start_position.y + Random.Range(-screenshakeinten, screenshakeinten));
                transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-0.8f, 0.8f));
            }
        }

        //Set Transform Position to Camera Lerp
        transform.position = new Vector3(Mathf.Round(start_position.x - (start_position.x % 0.03125f)), Mathf.Round(start_position.y - (start_position.y % 0.03125f)), transform.position.z);
	}

    //Public Functions
    /// <summary>
    /// setScreenShake(): sets the camera's screen shake system on to run for the amount of time and intensity given
    /// </summary>
    /// <param name="seconds"></param> how many seconds the screen shake should last
    /// <param name="intensity"></param> the intensity of the screen shake
    public void setScreenShake(float seconds, float intensity){
        screenshakedelay = 0;
        screenshaketime = seconds;
        screenshakeinten = intensity;
    }
}
