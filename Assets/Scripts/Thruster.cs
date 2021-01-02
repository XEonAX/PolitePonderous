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

    private Rigidbody spaceship;
    private float scale;

    public float getActualMaxPower => maxForce * scale;

    public Transform exhaust;
    private LineRenderer lineRenderer;
    private TrailRenderer trailRenderer;
    private ParticleSystem particleSystem;
    public Vector3 ForceVector;
    public Vector3 TorqueVector;

    private void Awake()
    {
        spaceship = GetComponentInParent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale.magnitude;
        // lineRenderer = GetComponent<LineRenderer>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        if (trailRenderer != null)
            trailRenderer.startWidth = 0.2f * transform.localScale.sqrMagnitude;
    }

    // Update is called once per frame
    void Update()
    {
        exhaust.localScale = new Vector3(exhaust.localScale.x, exhaust.localScale.y, currentPower * (maxForce / 100));
        // lineRenderer.SetPosition(0, parentRb.worldCenterOfMass);
        // lineRenderer.SetPosition(1, parentRb.worldCenterOfMass + Vector3.Cross(transform.position, transform.forward * maxForce));
    }

    void FixedUpdate()
    {
        power = Mathf.Clamp(power, 0, 1);
        float smoothingVelocity = 0;
        currentPower = Mathf.SmoothDamp(currentPower, power, ref smoothingVelocity, Time.fixedDeltaTime, ThrottleSpeed);
        spaceship.AddForceAtPosition(transform.forward * Mathf.Clamp(maxForce * currentPower, 0, maxForce) * scale, transform.position, ForceMode.Force);
        if (particleSystem != null)
        {
            var emission = particleSystem.emission;
            emission.rateOverTime = power * 50;
        }
        if (trailRenderer != null)
        {
            if (power > 0.3f)
            {
                trailRenderer.emitting = true;
            }
            else
                trailRenderer.emitting = false;
        }
        // if (power > 0.1f)
        // {
        //     particleSystem.Play();// = true;
        // }
        // else
        //     particleSystem.Play();// = false;
    }

    internal void InitializeThrustVector()
    {
        float FForward = Vector3.Dot(transform.forward, spaceship.transform.forward);
        float FRight = Vector3.Dot(transform.forward, spaceship.transform.right);
        float FUp = Vector3.Dot(transform.forward, spaceship.transform.up);

        var torqueCross = Vector3.Cross(transform.position - spaceship.worldCenterOfMass, transform.forward);
        float TForward = Vector3.Dot(torqueCross, spaceship.transform.forward);
        float TRight = Vector3.Dot(torqueCross, spaceship.transform.right);
        float TUp = Vector3.Dot(torqueCross, spaceship.transform.up);

        ForceVector = new Vector3(FRight, FUp, FForward) * (maxForce);
        TorqueVector = new Vector3(TRight, TUp, TForward) * (maxForce);
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawRay(parentRb.worldCenterOfMass, transform.position - parentRb.worldCenterOfMass);
    //     Gizmos.DrawRay(transform.position, transform.forward);
    //     Gizmos.DrawRay(parentRb.worldCenterOfMass, Vector3.Cross(transform.position - parentRb.worldCenterOfMass, transform.forward));
    // }
}
