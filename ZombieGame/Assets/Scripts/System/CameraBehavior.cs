using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	//Settings
    [SerializeField] float spd = 0.05f;
    private Vector2 start_position;
    private Vector2 end_position;

    private float screenshaketime;
    private float screenshaketotal;
    private float screenshakeinten;
    private float screenshakedelay;

	private GameObject player;
    private GameObject letterbox;
    private GameObject background;
    private BulletUI bulletUI_obj;

    //Init Event
	void Start () {
		start_position = transform.position;
        start_position = end_position;
		player = GameObject.FindGameObjectWithTag("Player");

        screenshaketime = 0f;
        letterbox = Instantiate(Resources.Load("pLetterBox") as GameObject, transform.position, transform.rotation);
        background = Instantiate(Resources.Load("pBackground") as GameObject, new Vector3(transform.position.x, transform.position.y, 500f), transform.rotation);
        bulletUI_obj = new GameObject("bulletUI").AddComponent<BulletUI>();
	}

	//Update Event
	void Update () {
        //Lerp Camera
        end_position = new Vector2(player.transform.position.x, LevelManager.instance.getLevelY(player.GetComponent<PlayerBehavior>().level) + 1f);
        start_position = new Vector2(Mathf.Lerp(start_position.x, end_position.x, Time.deltaTime * spd), Mathf.Lerp(start_position.y, end_position.y, Time.deltaTime * spd));
        transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        //Screen Shake
        if (screenshaketime > 0){
            screenshaketime -= Time.deltaTime;
            screenshakedelay -= Time.deltaTime;
            if (screenshakedelay <= 0){
                screenshakedelay = 0.1f;
                screenshakeinten = Mathf.Lerp(0, screenshakeinten, Mathf.Pow(screenshaketime / screenshaketotal, 2));
                start_position = new Vector2(start_position.x + Random.Range(-screenshakeinten, screenshakeinten), start_position.y + Random.Range(-screenshakeinten, screenshakeinten));
                transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-screenshakeinten, screenshakeinten));
            }
        }

        //Set Transform Position to Camera Lerp
        transform.position = new Vector3(start_position.x - (start_position.x % 0.03125f), start_position.y - (start_position.y % 0.03125f), transform.position.z);
        letterbox.transform.localEulerAngles = transform.localEulerAngles;
        letterbox.transform.position = new Vector3(start_position.x - (start_position.x % 0.03125f), start_position.y - (start_position.y % 0.03125f), transform.position.z + 2);
        background.transform.localEulerAngles = transform.localEulerAngles;
        background.transform.position = new Vector2(Mathf.Lerp(background.transform.position.x, transform.position.x, Time.deltaTime * 7), Mathf.Lerp(background.transform.position.y, transform.position.y, Time.deltaTime * 7));
        background.transform.position = new Vector3(background.transform.position.x - (background.transform.position.x % 0.03125f), background.transform.position.y - (background.transform.position.y % 0.03125f), 500f);
        bulletUI_obj.transform.localEulerAngles = transform.localEulerAngles;
        bulletUI_obj.transform.position = new Vector3(start_position.x - (start_position.x % 0.03125f), start_position.y - (start_position.y % 0.03125f), transform.position.z + 2);
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
        screenshaketotal = seconds;
        screenshakeinten = intensity;
    }

    public BulletUI bulletUI {
        get {
            return bulletUI_obj;
        }
    }
}

public class BulletUI : MonoBehaviour {

    //Settings
    private List<GameObject> ui;

    //Update
    void Update () {
        if (ui != null){
            if (ui.Count > 0){
                for (int i = ui.Count - 1; i >= 0; i--){
                    if (ui[i] != null){
                        Vector2 pos = new Vector2(transform.position.x + (i * 0.2f) - 7.25f, transform.position.y - 2.05f);
                        ui[i].transform.position = new Vector3(pos.x, pos.y, transform.position.z - 0.01f);
                        ui[i].transform.eulerAngles = transform.eulerAngles;
                    }
                }
            }
        }
    }

    //Public Methods
    public void setUI (int num, Sprite sprite) {
        deleteUI();
        ui = new List<GameObject>();
        for (int i = 0; i < num; i++){
            Vector2 pos = new Vector2(transform.position.x + (i * 0.2f) - 7.25f, transform.position.y - 2.05f);
            GameObject ui_blip = new GameObject("blip_" + i);
            ui_blip.AddComponent<SpriteRenderer>().sprite = sprite;
            ui_blip.transform.position = new Vector3(pos.x, pos.y, transform.position.z - 0.01f);
            ui_blip.transform.eulerAngles = transform.eulerAngles;
            ui_blip.transform.parent = transform;
            ui.Add(ui_blip);
        }
    }

    public void hideUI () {
        deleteUI();
    }

    //Private Functions
    private void deleteUI () {
        if (ui != null){
            if (ui.Count > 0){
                for (int i = ui.Count - 1; i >= 0; i--){
                    if (ui[i] != null){
                        Destroy(ui[i].gameObject);
                    }
                    ui.RemoveAt(i);
                }
            }
            ui = null;
        }
    }

}