using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    List<Asteroid> OriginalAsteroids;
    public List<Asteroid> Asteroids = new List<Asteroid>();
    public int Radius;
    public int Count;

    public static AsteroidField Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        OriginalAsteroids = GetComponentsInChildren<Asteroid>().ToList();
        Asteroids.AddRange(OriginalAsteroids);
        // for (int x = -Count; x < Count; x++)
        // {
        //     for (int y = -Count; y < Count; y++)
        //     {
        //         for (int z = -Count; z < Count; z++)
        //         {
        //             if (x != 0 && y != 0 && z != 0)
        //                 Asteroids.Add(Instantiate(OriginalAsteroids[Random.Range(0, OriginalAsteroids.Count)], new Vector3(x, y, z) * 2, Quaternion.identity, transform));
        //         }
        //     }
        // }
        // for (int i = 0; i < Count; i++)
        // {
        //     Asteroids.Add(Instantiate(OriginalAsteroids[Random.Range(0, OriginalAsteroids.Count)], Random.onUnitSphere * Radius, Random.rotationUniform, transform));
        // }
        for (int i = 0; i < Count; i++)
        {
            Vector3 pos = Random.insideUnitCircle.normalized;
            pos.z = pos.y;
            pos.y = 0;
            Asteroids.Add(Instantiate(OriginalAsteroids[Random.Range(0, OriginalAsteroids.Count)], pos * Radius * Random.Range(0.5f, 1.5f), Random.rotationUniform, transform));
        }
    }

}
