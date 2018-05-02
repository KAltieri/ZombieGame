using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	//Settings
    private static LevelManager getinstance;
    [SerializeField] private float[] levels;

    //Init
    void Awake() {
        getinstance = gameObject.GetComponent<LevelManager>();
    }

    //Update
    void LateUpdate() {
        
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
