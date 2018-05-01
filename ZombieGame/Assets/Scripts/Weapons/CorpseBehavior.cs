using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseBehavior : WeaponBehavior {

    //Settings
    private float food;

    protected override void init() {
        base.init();
        weapon_name = "Corpse";
        meele = false;
        meele_damage = 0;

        spd = 5f;
        ammo = 0;
        gameObject.tag = "Corpse";
        rb.freezeRotation = false;
        rb.gravityScale = 1;

        food = Random.Range(320, 520);
    }

    private void LateUpdate() {
        gameObject.layer = 11;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    public void eat () {
        food -= 1;

        if (food <= 0){
            Destroy(gameObject);
        }
    }

}
