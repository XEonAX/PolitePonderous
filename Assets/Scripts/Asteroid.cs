using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * (Random.Range(100, 5000)), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
