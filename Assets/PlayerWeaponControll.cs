using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(Camera))]
public class PlayerWeaponControll : MonoBehaviour
{
    Weapon weapon;
    Camera camera;
    [SerializeField] Health health;

    public static PlayerWeaponControll Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        weapon = GetComponent<Weapon>();
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000);
            }
            weapon.FireProjectile(targetPoint, camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth * 0.5f, -500, camera.nearClipPlane)), health.GetFaction());
        }
    }
}
