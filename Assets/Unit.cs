using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] MeshRenderer renderer;
    enemyWeaponController weaponControll;
    bool chasing = false;
    public int unitIndex { get; private set; }
    private void Awake()
    {
        weaponControll = GetComponent<enemyWeaponController>();
    }

    private void Update()
    {
        if (GameMaster.Instance.GetDistanceFromPlayer(unitIndex) < 30)
        {
            chasing = true;
            weaponControll.SetMayFire(true);
        }
        else
        {
            weaponControll.SetMayFire(false);
            //GameMaster.Instance.SetStopChase(unitIndex);
        }
        if (chasing)
        {
            GameMaster.Instance.SetTargetsPlayer(unitIndex);
        }
    }
    public void Select()
    {
        renderer.material.color = Color.red;
    }

    public void OnHit(Projectile projectile)
    {
        GameMaster.Instance.DestroyUnit(this);
    }

    public void SetIndex(int index) => unitIndex = index;


}
