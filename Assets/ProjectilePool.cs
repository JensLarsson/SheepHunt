using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    List<Projectile> activeProjectileList = new List<Projectile>();
    Stack<Projectile> projectilePool = new Stack<Projectile>();

    public static ProjectilePool Instance;
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateProjectiles();
    }
    void UpdateProjectiles()
    {
        Quaternion playerRotation = PlayerWeaponControll.Instance.transform.rotation;
        for (int i = activeProjectileList.Count - 1; i >= 0; i--)
        {
            activeProjectileList[i].position += activeProjectileList[i].velocity * Time.deltaTime;
            activeProjectileList[i].transform.position = activeProjectileList[i].position;
            activeProjectileList[i].transform.rotation = playerRotation;
        }
    }

    public Projectile GetProjectile(Projectile projectile, FACTION faction)
    {
        Projectile newProjectile;
        if (projectilePool.Count > 0)
        {
            newProjectile = projectilePool.Pop();
            activeProjectileList.Add(newProjectile);
            newProjectile.gameObject.SetActive(true);
        }
        else
        {
            newProjectile = Instantiate(projectile, this.transform);
            activeProjectileList.Add(newProjectile);
        }
        newProjectile.SetFaction(faction);
        return newProjectile;
    }

    public void PoolAndDeactivateProjectile(Projectile projectile)
    {
        activeProjectileList.Remove(projectile);
        projectilePool.Push(projectile);
        projectile.gameObject.SetActive(false);
    }

}
