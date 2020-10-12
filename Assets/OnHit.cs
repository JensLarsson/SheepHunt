using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHit : MonoBehaviour
{
    [SerializeField] MeshRenderer renderer;
    [SerializeField] Material OnHitMaterial;

    public void Hit()
    {
        StartCoroutine(DamageAnimation());
    }

    IEnumerator DamageAnimation()
    {
        renderer.material.color = Color.red;
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime;
            yield return null;
        }
        renderer.material.color = Color.white;
    }
}
