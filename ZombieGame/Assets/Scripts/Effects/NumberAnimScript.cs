using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberAnimScript : MonoBehaviour {

	//Settings
    private Animator anim_ones;
    private Animator anim_tens;
    private bool slow_count;
    private int current_count;
    private int target_count;

    private int delay;
    private int delay_timer;

    //Init
	void Awake () {
		anim_ones = transform.GetChild(0).gameObject.GetComponent<Animator>();
        anim_tens = transform.GetChild(1).gameObject.GetComponent<Animator>();
        setNumber(0);
        slow_count = false;

        delay = 0;
        delay_timer = 0;
	}
	
	//Update
	void Update () {
        //Slow Count
        Vector2 shake_offset = Vector2.zero;
        if (slow_count) {
            if (current_count < target_count) {
                delay_timer--;
                if (delay_timer <= 0) {
                    current_count++;
                    current_count = Mathf.Clamp(current_count, 0, target_count);
                    setNumber(current_count);
                    float shake_inten = 0.15f;
                    shake_offset = new Vector2(Random.Range(-shake_inten, shake_inten), Random.Range(-shake_inten, shake_inten));

                    delay = Mathf.Clamp(delay / 2, 5, delay / 2);
                    delay_timer = delay;
                }
            }
        }

        //Draw Count
        float offset = 0;
        if (!anim_tens.GetComponent<SpriteRenderer>().enabled) {
            offset = -0.65f;
        }
		transform.GetChild(0).gameObject.transform.position = new Vector3(transform.position.x + 1.15f + offset + shake_offset.x, transform.position.y + shake_offset.y, transform.position.z);
        transform.GetChild(1).gameObject.transform.position = new Vector3(transform.position.x + shake_offset.x, transform.position.y + shake_offset.y, transform.position.z);

        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
	}

    //Public Functions
    public void setNumber(int num) {
        if (num == 0){
            anim_ones.Play("number_0");
            anim_tens.GetComponent<SpriteRenderer>().enabled = false;
            return;
        }
        int ones = num % 10;
        int tens = (num - ones) / 10;

        anim_ones.Play("number_" + ones);
        anim_tens.Play("number_" + tens);
        if (tens > 0){
            anim_tens.GetComponent<SpriteRenderer>().enabled = true;
        }
        else {
            anim_tens.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void slowCount(int num) {
        slow_count = true;
        setNumber(0);
        current_count = 0;
        target_count = num;

        delay = 60;
        delay_timer = 120;
    }

    public bool finishedCounting() {
        if (slow_count) {
            if (current_count == target_count) {
                return true;
            }
        }
        return false;
    }

}
