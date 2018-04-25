
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBehavior : InteractInterface {

    //Settings
    protected SpriteRenderer sr;
    private SpriteOutline sprout;
    protected Collider2D col;

    protected bool selected;
    private float alpha;

    //Internals
    protected void Start() {
        selected = false;
        sr = GetComponent<SpriteRenderer>();
        sr.material = Resources.Load("Materials/PixelPerfectOutline") as Material;
        sprout = gameObject.AddComponent<SpriteOutline>();
        col = GetComponent<Collider2D>();
        gameObject.AddComponent<PixelSnap>();
        init();
    }

    protected void Update() {
        step();
    }

    //Initialization & Update Methods
    protected override void init() {
        alpha = 0;
        sprout.OutlineColor = new Color(1, 1, 1, 0);
    }

    protected override void step() {
        selected = false;
        PlayerBehavior player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();

        bool alpha_drop = true;
        if (player.canmove){
            if (col.bounds.Contains(new Vector3(player.getCursorPosition().x, player.getCursorPosition().y, transform.position.z))){
                alpha = Mathf.Lerp(alpha, 1, Time.deltaTime * 5f);
                alpha_drop = false;
                selected = true;
            }
        }

        if (alpha_drop){
            alpha = Mathf.Lerp(alpha, 0, Time.deltaTime * 5f);
        }

        alpha = Mathf.Clamp(alpha, 0, 1);
        sprout.OutlineColor = new Color(1, 1, 1, alpha * 0.8f);
    }

}
