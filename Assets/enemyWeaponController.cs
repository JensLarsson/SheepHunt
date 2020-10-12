using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(Health))]
public class enemyWeaponController : MonoBehaviour
{
    [SerializeField] float shotsPerMinute = 20;
    float fireCoolDown = 0;
    bool mayFire = false;
    Health health;
    Weapon weapon;
    // Start is called before the first frame update
    void Awake()
    {
        weapon = GetComponent<Weapon>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (mayFire)
        {
            fireCoolDown += Time.deltaTime;

            if (fireCoolDown > 60 / shotsPerMinute)
            {
                fireCoolDown = 0;
                weapon.FireProjectile(GameMaster.Instance.GetPlayerPosition(), transform.position, health.GetFaction());
            }
        }
    }
    public void SetMayFire(bool b) => mayFire = b;
}
