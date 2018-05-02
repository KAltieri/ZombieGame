using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    //Menus
    private GameObject main_menu;
    private GameObject game_over;
    private GameObject victory;
    private bool victory_countstart;
    private bool reset;
    private float reset_highscore;

    //Init Event
	void Start () {
		start_position = transform.position;
        end_position = start_position;
		player = GameObject.FindGameObjectWithTag("Player");

        screenshaketime = 0f;
        letterbox = Instantiate(Resources.Load("pLetterBox") as GameObject, transform.position, transform.rotation);
        background = Instantiate(Resources.Load("pBackground") as GameObject, new Vector3(transform.position.x, transform.position.y, 500f), transform.rotation);
        bulletUI_obj = new GameObject("bulletUI").AddComponent<BulletUI>();
        
        main_menu = Instantiate(Resources.Load("Menus/pMainMenu") as GameObject, new Vector3(transform.position.x, transform.position.y, -9.5f), transform.rotation);
        main_menu.transform.GetChild(0).transform.position = new Vector3(transform.position.x, transform.position.y + 5f, -9.5f);
        main_menu.transform.GetChild(1).transform.position = new Vector3(transform.position.x, transform.position.y - 7f, -9.5f);
        main_menu.transform.GetChild(2).transform.position = new Vector3(transform.position.x, transform.position.y - 1.3f, -9.5f);
        main_menu.transform.GetChild(3).transform.position = new Vector3(transform.position.x + 1.4f, transform.position.y - 1.3f, -9.5f);
        main_menu.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        main_menu.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        main_menu.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        main_menu.transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

        victory_countstart = false;
        if (!PlayerPrefs.HasKey("highscore")) {
            PlayerPrefs.SetInt("highscore", 0);
        }
        else {
            main_menu.transform.GetChild(3).gameObject.GetComponent<NumberAnimScript>().setNumber(PlayerPrefs.GetInt("highscore"));
        }
        reset = true;
        reset_highscore = 0;
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

    //Menus
    void LateUpdate() {
        PlayerBehavior player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
        if (main_menu != null){
            float reset_shake = reset_highscore * 0.65f;
            main_menu.transform.GetChild(0).transform.position = new Vector3(transform.position.x, Mathf.Lerp(main_menu.transform.GetChild(0).transform.position.y, end_position.y, Time.deltaTime * 3f), -9.5f);
            main_menu.transform.GetChild(1).transform.position = new Vector3(transform.position.x, Mathf.Lerp(main_menu.transform.GetChild(1).transform.position.y, end_position.y, Time.deltaTime * 1.5f), -9.5f);
            main_menu.transform.GetChild(2).transform.position = new Vector3(transform.position.x, transform.position.y - 1.3f, -9.5f);
            main_menu.transform.GetChild(3).transform.position = new Vector3(transform.position.x + 1.4f + Random.Range(-reset_shake, reset_shake), transform.position.y - 1.3f + Random.Range(-reset_shake, reset_shake), -9.5f);
            if (!player.canmove){
                main_menu.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(0).GetComponent<SpriteRenderer>().color.a, 1, Time.deltaTime * 0.55f));
                main_menu.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(1).GetComponent<SpriteRenderer>().color.a, 1, Time.deltaTime * 0.35f));
                if (Mathf.Abs(main_menu.transform.GetChild(1).transform.position.y - end_position.y) < 0.5f) {
                    if (Vector2.Distance(player.getCursorPosition(), new Vector2(6.25f + transform.position.x, -4.15f + transform.position.y)) < 1.2f) {
                        main_menu.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(2).GetComponent<SpriteRenderer>().color.a, 1, Time.deltaTime * 5f));
                        main_menu.transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(3).GetComponent<SpriteRenderer>().color.a, 1, Time.deltaTime * 5f));
                        if (reset) {
                            if (Input.GetKey(KeyCode.R)) {
                                reset_highscore += 0.0047f;
                                if (reset_highscore >= 1) {
                                    reset = false;
                                    reset_highscore = 0;
                                    PlayerPrefs.SetInt("highscore", 0);
                                    main_menu.transform.GetChild(3).gameObject.GetComponent<NumberAnimScript>().setNumber(0);
                                }
                            }
                            else {
                                if (reset_highscore > 0) {
                                    reset_highscore -= 0.01f;
                                }
                            }
                            reset_highscore = Mathf.Clamp(reset_highscore, 0, 1);
                        }
                    }
                    else {
                        main_menu.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(2).GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5f));
                        main_menu.transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(3).GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5f));
                    }
                }
            }
            else {
                main_menu.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(0).GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5f));
                main_menu.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(1).GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5f));
                main_menu.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(2).GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5f));
                main_menu.transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(main_menu.transform.GetChild(3).GetComponent<SpriteRenderer>().color.a, 0, Time.deltaTime * 5f));
                if (main_menu.transform.GetChild(0).GetComponent<SpriteRenderer>().color.a <= 0.01f){
                    Destroy(main_menu.gameObject);
                }
            }

            if (Input.GetMouseButtonDown(0)){
                player.startGame();
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                Application.Quit();
            }
        }
        if (game_over != null){
            game_over.transform.GetChild(0).transform.position = new Vector3(transform.position.x, transform.position.y, -9.5f);
            game_over.transform.GetChild(1).transform.position = new Vector3(transform.position.x, transform.position.y, -9.5f);
            game_over.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(game_over.transform.GetChild(0).GetComponent<SpriteRenderer>().color.a, 1, Time.deltaTime * 3f));
            game_over.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Mathf.Lerp(game_over.transform.GetChild(1).GetComponent<SpriteRenderer>().color.a, 1, Time.deltaTime * 3f));

            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene("Level");
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                Application.Quit();
            }
        }
        if (victory) {
            victory.transform.GetChild(0).transform.position = new Vector3(transform.position.x, transform.position.y, -9.5f);
            victory.transform.GetChild(1).transform.position = new Vector3(transform.position.x, transform.position.y - 2.7f, -9.5f);
            victory.transform.GetChild(2).transform.position = new Vector3(transform.position.x + 1.25f, transform.position.y, -9.5f);
            float alpha = victory.transform.GetChild(0).GetComponent<SpriteRenderer>().color.a;
            if (alpha > 0.0005f) {
                alpha = Mathf.Lerp(alpha, 1, Time.deltaTime * 1f);   
                if  (alpha > 0.85f) {
                    if (!victory_countstart) {
                        victory_countstart = true;
                        victory.transform.GetChild(2).gameObject.GetComponent<NumberAnimScript>().slowCount(player.getEvacHumanCount());
                    }
                }
            }
            else {
                alpha += 0.000005f;
            }
            victory.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
            victory.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
            if (victory.transform.GetChild(2).gameObject.GetComponent<NumberAnimScript>().finishedCounting()) {
                victory.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(victory.transform.GetChild(1).GetComponent<SpriteRenderer>().color.a, 1, Time.deltaTime * 0.85f));
                if (Input.GetKeyDown(KeyCode.R)) {
                    SceneManager.LoadScene("Level");
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                Application.Quit();
            }
        }
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

    public void createMenu(string menu_type) {
        if (menu_type == "gameover"){
            game_over = Instantiate(Resources.Load("Menus/pGameOver") as GameObject, new Vector3(transform.position.x, transform.position.y, -9.5f), transform.rotation);
            game_over.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            game_over.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
        else if (menu_type == "victory"){
            victory = Instantiate(Resources.Load("Menus/pVictory") as GameObject, new Vector3(transform.position.x, transform.position.y, -9.5f), transform.rotation);
            victory.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            victory.transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            victory.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
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