using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Obstacle_Cylinder : MonoBehaviour
{
    public static List<Obstacle_Cylinder> obstacles = new List<Obstacle_Cylinder>();
    void Awake()
    {
        obstacles.Add(this);
    }
    private void OnDisable()
    {
        obstacles.Remove(this);
    }
    public float Radius => this.transform.lossyScale.x * 0.5f;

    /// <summary>
    /// Returns a Vector4 containing (x,y,z,Radius)
    /// </summary>
    /// <returns></returns>
    public Vector4 GetPositionAndRadious()
    {
        Vector3 pos = transform.position;
        return new Vector4(pos.x, pos.y, pos.z, Radius);
    }

}
