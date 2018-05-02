using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	//Settings
    private static LevelManager getinstance;
    [SerializeField] private float[] levels;

    private float volume;
    private AudioSource mus_loop;
    private List<AudioSource> mus;
    private int bark_timer;

    //Init
    void Awake() {
        getinstance = gameObject.GetComponent<LevelManager>();

        volume = 0.7f;
        mus = new List<AudioSource>();
        mus_loop = gameObject.AddComponent<AudioSource>();
        bark_timer = 500;
    }

    //Update
    void LateUpdate() {
        if (!mus_loop.loop) {
            bark_timer--;
            if (bark_timer <= 0) {
                mus_loop.clip = (AudioClip) Resources.Load("SFX/ZombieBarkSFX");
                mus_loop.Play();
                bark_timer = Random.Range(120, 380);
            }
        }

        mus_loop.volume = volume * 0.15f;
        if (mus.Count > 0){
            for (int i = mus.Count - 1; i >= 0; i--){
                mus[i].volume = volume;
                if (!mus[i].isPlaying){
                    AudioSource aud = mus[i];
                    mus.Remove(aud);
                    Destroy(aud);
                }
            }
        }
    }

    //Public Methods
    /// <summary>
    /// getLevelY(): returns the level y position of the level argument
    /// level: the level you want the y position of
    /// </summary>
    public float getLevelY(int level) {
        return levels[level];
    }

    public int getLevelNum(float y_pos){
        int return_num = 0;
        float distance = Mathf.Abs(y_pos - levels[0]);
        for (int i = 0; i < levels.Length; i++){
            if (Mathf.Abs(y_pos - levels[i]) < distance){
                return_num = i;
                distance = Mathf.Abs(y_pos - levels[i]);
            }
        }
        return return_num;
    }

    public void playMusic(string sfx_name) {
        AudioSource aud = gameObject.AddComponent<AudioSource>();
        aud.clip = (AudioClip) Resources.Load("SFX/" + sfx_name);
        aud.volume = volume;
        mus.Add(aud);
        aud.Play();
    }

    public void playMusicLoop(string sfx_name) {
        mus_loop.loop = true;
        mus_loop.clip = (AudioClip) Resources.Load("SFX/" + sfx_name);
        mus_loop.volume = volume;
        mus_loop.Play();
    }

    public float getVolume() {
        return volume;
    }

    /// <summary>
    /// instance: returns LevelManager instance
    /// You can use this to call the level manager whenever in order to use the public methods shown here
    /// Example)
    ///     LevelManager.instance.[MethodGoesHere()];
    /// </summary>
    public static LevelManager instance {
        get {
            return getinstance;
        }
    }

    //Debug
    void OnDrawGizmos () {
        Gizmos.color = Color.red;
            if (levels.Length > 0){
            for (int i = 0; i < levels.Length; i++){
                Gizmos.DrawLine(new Vector3(-5f, levels[i], 0f), new Vector3(5f, levels[i], 0f));
            }
        }
    }
}
