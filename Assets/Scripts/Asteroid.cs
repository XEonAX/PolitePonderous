using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    Rigidbody rb;
    private float F;
    private float V = 10;

    // Start is called before the first frame update
    void Start()
    {
        // GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * (Random.Range(100, 5000)), ForceMode.Impulse);
        //StartCoroutine(PrepareBoom());
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.Cross(transform.position.normalized, Vector3.up) * V, ForceMode.VelocityChange);
        F = (rb.mass * V * V) / transform.position.magnitude;
    }
    IEnumerator PrepareBoom()
    {
        yield return new WaitForSeconds(Random.Range(10, 100));
        rb.AddTorque(Random.onUnitSphere * (Random.Range(100, 500)), ForceMode.Impulse);
        StartCoroutine(GoBoom());
    }
    IEnumerator GoBoom()
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        //rb.AddForce(-transform.position.normalized * (Random.Range(1000, 50000)), ForceMode.Impulse);
        StartCoroutine(PrepareBoom());
    }

    // Update is called once per frame
    // void FixedUpdate()
    // {
    //     // rb.AddForce(-transform.position, ForceMode.Impulse);
    // }
    // private void OnCollisionEnter(Collision other)
    // {
    //     gameObject.SetActive(false);
    //     AsteroidField.Instance.Asteroids.Remove(this);
    // }

    private void FixedUpdate()
    {
        rb.AddForce(-transform.position.normalized * F, ForceMode.Force);
    }
}
