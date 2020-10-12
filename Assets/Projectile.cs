using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 40;
    [SerializeField] int damage = 100;
    [SerializeField] float lifeTimeToDeath = 60;
    public Vector3 position;
    public Vector3 velocity;
    FACTION faction;
    float lifeTime;

    private void OnEnable()
    {
        lifeTime = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        Health unit = other.gameObject.GetComponent<Health>();
        if (unit != null && unit.GetFaction() != faction)
        {
            unit.DealDamage(damage);
            PoolAndDeactivateProjectile();
        }
    }
    private void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > lifeTimeToDeath)
        {
            PoolAndDeactivateProjectile();
        }
    }
    public void SetFaction(FACTION fac)
    {
        faction = fac;
    }

    void PoolAndDeactivateProjectile()
    {
        ProjectilePool.Instance.PoolAndDeactivateProjectile(this);
    }
}
