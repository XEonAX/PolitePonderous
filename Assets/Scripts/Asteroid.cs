using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * (Random.Range(100, 5000)), ForceMode.Impulse);
        StartCoroutine(PrepareBoom());
    }
    IEnumerator PrepareBoom()
    {
        yield return new WaitForSeconds(Random.Range(10, 100));
        GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * (Random.Range(100, 500)), ForceMode.Impulse);
        StartCoroutine(GoBoom());
    }
    IEnumerator GoBoom()
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        //GetComponent<Rigidbody>().AddForce(-transform.position.normalized * (Random.Range(1000, 50000)), ForceMode.Impulse);
        StartCoroutine(PrepareBoom());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // GetComponent<Rigidbody>().AddForce(-transform.position, ForceMode.Impulse);
    }
}
