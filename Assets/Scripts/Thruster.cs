using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    public float maxForce;

    public float power;
    private float currentPower;

    public float ThrottleSpeed;

    private Rigidbody parentRb;
    private float scale;

    public float getActualMaxPower => maxForce * scale;

    public Transform exhaust;
    private LineRenderer lineRenderer;
    private TrailRenderer trailRenderer;
    private ParticleSystem particleSystem;
    private void Awake()
    {
        parentRb = GetComponentInParent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale.magnitude;
        // lineRenderer = GetComponent<LineRenderer>();
        // trailRenderer = GetComponentInChildren<TrailRenderer>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        //trailRenderer.startWidth = 0.2f * transform.localScale.sqrMagnitude;
    }

    // Update is called once per frame
    void Update()
    {
        exhaust.localScale = new Vector3(exhaust.localScale.x, exhaust.localScale.y, currentPower * (maxForce / 50));
        // lineRenderer.SetPosition(0, parentRb.worldCenterOfMass);
        // lineRenderer.SetPosition(1, parentRb.worldCenterOfMass + Vector3.Cross(transform.position, transform.forward * maxForce));
    }

    void FixedUpdate()
    {
        power = Mathf.Clamp(power, 0, 1);
        currentPower = Mathf.Lerp(currentPower, power, Time.fixedDeltaTime * ThrottleSpeed);
        parentRb.AddForceAtPosition(transform.forward * Mathf.Clamp(maxForce * currentPower, 0, maxForce) * scale, transform.position, ForceMode.Force);
        var emission = particleSystem.emission;
        emission.rateOverTime = power * 100;
        // if (power > 0.1f)
        // {
        //     particleSystem.Play();// = true;
        // }
        // else
        //     particleSystem.Play();// = false;
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawRay(parentRb.worldCenterOfMass, transform.position - parentRb.worldCenterOfMass);
    //     Gizmos.DrawRay(transform.position, transform.forward);
    //     Gizmos.DrawRay(parentRb.worldCenterOfMass, Vector3.Cross(transform.position - parentRb.worldCenterOfMass, transform.forward));
    // }
}
