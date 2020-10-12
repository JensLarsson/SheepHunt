using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Projectile projectilePrefab;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void FireProjectile(Vector3 target, Vector3 spawnPosition, FACTION faction)
    {
        Projectile projectile = ProjectilePool.Instance.GetProjectile(projectilePrefab, faction);

        //projectile.position = transform.position + transform.forward;
        projectile.position = spawnPosition;
        projectile.transform.position = projectile.position;
        projectile.velocity = (target - projectile.position).normalized * projectile.speed;
    }
}
