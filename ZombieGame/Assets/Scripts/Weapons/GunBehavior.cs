using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehavior : WeaponBehavior {

    [SerializeField] public int bullet_num = 1;
    [SerializeField] public int bullet_spread = 15;
    [SerializeField] public float bullet_force = 2500f;
    [SerializeField] public float bullet_screen_shake_time = 0.8f;
    [SerializeField] public float bullet_screen_shake_force = 0.15f;
    [SerializeField] public float bullet_damage = 0.35f;
    [SerializeField] public Sprite bullet_sprite;
    [SerializeField] public Sprite bulletUI_sprite;

    protected override void init() {
        base.init();
        meele = false;
    }

    protected override void step(){
        if (!player){
            base.step();
            if (player){
                Camera.main.GetComponent<CameraBehavior>().bulletUI.setUI(ammo, bulletUI_sprite);
            }
        }
        else {
            base.step();
        }
    }

    protected override void use(){
        base.use();

        //SFX
        if (weapon_name == "SMG") {
            int sfx_num = Random.Range(1, 6);
            LevelManager.instance.playMusic("SMG" + sfx_num + "SFX");
        }
        else if (weapon_name == "Rifle"){
            LevelManager.instance.playMusic("RifleSFX");
            LevelManager.instance.playMusic("ShotgunSFX");
            LevelManager.instance.playMusic("PistolSFX");
        }
        else if (weapon_name == "Pistol"){
            LevelManager.instance.playMusic("PistolSFX");
        }
        else if (weapon_name == "Shotgun"){
            LevelManager.instance.playMusic("ShotgunSFX");
        }

        Camera.main.GetComponent<CameraBehavior>().bulletUI.setUI(ammo, bulletUI_sprite);
        for (int i = 0; i < bullet_num; i++){
            GameObject bullet = new GameObject(weapon_name + "_bullet");
            bullet.gameObject.layer = 11;
            bullet.transform.position = transform.position;
            bullet.AddComponent<SpriteRenderer>();
            bullet.GetComponent<SpriteRenderer>().sprite = bullet_sprite;
            bullet.AddComponent<Rigidbody2D>();
            bullet.GetComponent<Rigidbody2D>().gravityScale = 0;
            bullet.AddComponent<BoxCollider2D>();
            Physics2D.IgnoreCollision(bullet.GetComponent<BoxCollider2D>(), ignore_collider);

            float bullet_angle = ((use_angle * Mathf.Rad2Deg) + Random.Range(-bullet_spread, bullet_spread)) * Mathf.Deg2Rad;
            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(bullet_angle) * bullet_force, Mathf.Sin(bullet_angle) * bullet_force));
            bullet.transform.eulerAngles = new Vector3(0f, 0f, bullet_angle * Mathf.Rad2Deg);

            bullet.AddComponent<TrailEffectScript>();
            bullet.AddComponent<BulletScript>();
            bullet.GetComponent<BulletScript>().damage = bullet_damage;
        }

        Camera.main.GetComponent<CameraBehavior>().setScreenShake(bullet_screen_shake_force, bullet_screen_shake_force);
    }

}
