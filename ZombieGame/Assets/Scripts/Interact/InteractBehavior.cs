
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBehavior : InteractInterface {

    //Settings
    private SpriteRenderer sr;
    private SpriteOutline sprout;
    private Collider2D col;

    //Internals
    protected void Start() {
        sr = GetComponent<SpriteRenderer>();
        sr.material = Resources.Load("Materials/PixelPerfectOutline") as Material;
        sprout = gameObject.AddComponent<SpriteOutline>();
        sprout.OutlineColor = new Color(1, 1, 1, 0);
        col = GetComponent<Collider2D>();
        gameObject.AddComponent<PixelSnap>();
        init();
    }

    protected void Update() {
        step();
    }

    //Initialization & Update Methods
    protected override void init() {

    }

    protected override void step() {

    }

}
