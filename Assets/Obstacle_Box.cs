using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Obstacle_Box : MonoBehaviour
{
    public static List<Obstacle_Box> obstacles = new List<Obstacle_Box>();
    void Awake()
    {
        obstacles.Add(this);
    }
    private void OnDisable()
    {
        obstacles.Remove(this);
    }

    public Vector3 GetPosition => transform.position;
    public Vector3 GetCenterWidth => transform.lossyScale*0.5f;

    public Vector6 GetBox => new Vector6(GetPosition, GetCenterWidth);
}
