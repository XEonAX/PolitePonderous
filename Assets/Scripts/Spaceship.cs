using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spaceship : MonoBehaviour
{
    public Rigidbody rb;
    public List<Thruster> thrusters;
    public List<Thruster> mainThrusters;
    public List<Thruster> reverseThrusters;
    public List<Thruster> leftlateralThrusters;
    public List<Thruster> rightlateralThrusters;
    public List<Thruster> dorsalThrusters;
    public List<Thruster> ventralThrusters;

    public Transform CameraPosition;
    public Transform CoM;

    public InputMgr InputMgr;
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        BootUpThrusters();
        CoM.localPosition = rb.centerOfMass;
    }

    private void BootUpThrusters()
    {
        thrusters = GetComponentsInChildren<Thruster>().ToList();
        mainThrusters = thrusters.Where(x => Vector3.Dot(x.transform.forward, transform.forward) > 0.9f).ToList();
        reverseThrusters = thrusters.Where(x => Vector3.Dot(x.transform.forward, -transform.forward) > 0.9f).ToList();
        dorsalThrusters = thrusters.Where(x => Vector3.Dot(x.transform.forward, -transform.up) > 0.9f).ToList();
        ventralThrusters = thrusters.Where(x => Vector3.Dot(x.transform.forward, transform.up) > 0.9f).ToList();
        leftlateralThrusters = thrusters.Where(x => Vector3.Dot(x.transform.forward, transform.right) > 0.9f).ToList();
        rightlateralThrusters = thrusters.Where(x => Vector3.Dot(x.transform.forward, -transform.right) > 0.9f).ToList();


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        var expectedVelocity = transform.TransformDirection(new Vector3(InputMgr.vLeftRight, InputMgr.vUpDown, InputMgr.vForwardBack));
        var expectedAngular = (transform.forward * InputMgr.vRoll);
        expectedAngular += -transform.right * InputMgr.vAim.y;
        expectedAngular += transform.up * InputMgr.vAim.x;
        if (!InputMgr.disableStabilizer)
        {
            if (expectedVelocity.sqrMagnitude <= 0.2f)
            {
                expectedVelocity = (-rb.velocity) * InputMgr.StabilizerCurve.Evaluate(rb.velocity.sqrMagnitude);
                // if (rb.velocity.sqrMagnitude > 1)
                //     expectedVelocity = (-rb.velocity).normalized * 10;
                // else
                //     expectedVelocity = (-rb.velocity).normalized;
            }
            else
            {
                if (Vector3.Dot(expectedVelocity, rb.velocity) < 0)
                {
                    expectedVelocity = -rb.velocity + expectedVelocity;
                }
            }
            if (expectedAngular.sqrMagnitude <= 0.2f)
            {
                expectedAngular = (-rb.angularVelocity) * InputMgr.StabilizerCurve.Evaluate(rb.angularVelocity.sqrMagnitude);
                // if (rb.angularVelocity.sqrMagnitude > 0.5f)
                //     expectedAngular = (-rb.angularVelocity).normalized * 100;
                // else
                //     expectedAngular = (-rb.angularVelocity).normalized * rb.angularVelocity.sqrMagnitude;
            }
            else
            {
                if (Vector3.Dot(expectedAngular, rb.angularVelocity) < 0)
                {
                    expectedAngular = -rb.angularVelocity + expectedAngular;
                }
            }
        }
        foreach (Thruster thruster in this.thrusters)
        {
            float dotV = Vector3.Dot(thruster.transform.forward, expectedVelocity);
            if (dotV > 0.7f)
            {
                thruster.power = Mathf.InverseLerp(0.7f, 1, dotV);
            }
            else
            {
                thruster.power = 0;
            }

            float dotA = Vector3.Dot(Vector3.Cross(thruster.transform.position - rb.worldCenterOfMass, thruster.transform.forward), expectedAngular);
            // float dotA = Vector3.Dot(Vector3.Cross(thruster.transform.position - rb.worldCenterOfMass, thruster.transform.forward), expectedAngular);
            //thruster.power += Mathf.InverseLerp(-1, 1, dotA);
            if (dotA > 0.1f)
            {
                thruster.power += Mathf.InverseLerp(0.1f, 1, dotA);
            }
        }
        // // rb.AddRelativeForce(Vector3.forward*vForwardBack);
        // // rb.AddRelativeForce(Vector3.right*vLeftRight);
        // // rb.AddRelativeForce(Vector3.up*vUpDown);
        // // rb.AddRelativeTorque(Vector3.forward*vRoll);
        // // rb.AddRelativeTorque(0.005f*vAim.x*Vector3.up);
        // // rb.AddRelativeTorque(0.005f*vAim.y*Vector3.left);
        // vAim *= 0.999f;
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawRay(transform.position, expectedAngular);
    // }
}
