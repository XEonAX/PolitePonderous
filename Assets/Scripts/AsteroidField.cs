using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    List<Asteroid> OriginalAsteroids;
    public int Radius;
    public int Count;
    // Start is called before the first frame update
    void Start()
    {
        OriginalAsteroids = GetComponentsInChildren<Asteroid>().ToList();
        for (int i = 0; i < Count; i++)
        {
            Instantiate(OriginalAsteroids[Random.Range(0, OriginalAsteroids.Count)], Random.insideUnitSphere * Radius, Random.rotationUniform, transform);
        }
        for (int i = 0; i < Count; i++)
        {
            Instantiate(OriginalAsteroids[Random.Range(0, OriginalAsteroids.Count)], Random.onUnitSphere * Radius, Random.rotationUniform, transform);
        }
    }

}
