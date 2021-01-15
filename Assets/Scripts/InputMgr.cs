using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputMgr : MonoBehaviour
{
    public float vForwardBack;
    public float vForwardBackIncrementor;
    public bool vForwardBackDoIncrement;

    public float vLeftRight;
    public float vLeftRightIncrementor;
    public bool vLeftRightDoIncrement;
    public float vUpDown;
    public float vUpDownIncrementor;
    public bool vUpDownDoIncrement;
    public float vRoll;
    public float vRollIncrementor;
    public bool vRollDoIncrement;
    public Vector2 vAim;
    public bool disableStabilizer = false;
    public AnimationCurve StabilizerCurve;
    public AnimationCurve AimSensitivityCurve;
    public Vector3 expectedVelocity;
    public Vector3 expectedAngular;
    public static InputMgr Instance;

    public bool PrimaryFire;
    public bool SecondaryFire;
    private void Awake()
    {
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnForwardBack(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
        {
            vForwardBackIncrementor = value.ReadValue<float>();
            vForwardBackDoIncrement = true;
        }
        else if (value.phase == InputActionPhase.Canceled)
            vForwardBackDoIncrement = false;
    }
    public void OnLeftRight(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
        {
            vLeftRightIncrementor = value.ReadValue<float>();
            vLeftRightDoIncrement = true;
        }
        else if (value.phase == InputActionPhase.Canceled)
            vLeftRightDoIncrement = false;
    }
    public void OnUpDown(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
        {
            vUpDownIncrementor = value.ReadValue<float>();
            vUpDownDoIncrement = true;
        }
        else if (value.phase == InputActionPhase.Canceled)
            vUpDownDoIncrement = false;
    }
    public void OnRoll(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
        {
            vRollIncrementor = value.ReadValue<float>();
            vRollDoIncrement = true;
        }
        else if (value.phase == InputActionPhase.Canceled)
            vRollDoIncrement = false;
    }
    public void OnAim(InputAction.CallbackContext value)
    {
        var vAimDelta = value.ReadValue<Vector2>();
        //Debug.Log(vAim.sqrMagnitude + "  " + vAimDelta);
        vAim += vAimDelta * 0.005f;
        vAim = Vector3.ClampMagnitude(vAim, 1);
    }
    public void OnResetForwardBack(InputAction.CallbackContext value)
    {
        vForwardBack = 0;
        vForwardBackIncrementor = 0;
        vLeftRight = 0;
        vLeftRightIncrementor = 0;
        vUpDown = 0;
        vUpDownIncrementor = 0;
        vRoll = 0;
        vRollIncrementor = 0;
        vAim = Vector2.zero;
    }
    public void OnResetRoll(InputAction.CallbackContext value)
    {
        vRoll = 0;
        vRollIncrementor = 0;
    }

    public void OnDisableStabilizer(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
        {
            disableStabilizer = true;
            // foreach (var item in Physics.OverlapSphere(SpaceShipManager.Instance.FocusedSpaceship.transform.position, 100))
            // {
            //     if (item.GetComponent<Asteroid>() != null)
            //         item.GetComponent<Rigidbody>().AddExplosionForce(10000,
            //         SpaceShipManager.Instance.FocusedSpaceship.transform.position,
            //         100, 0, ForceMode.Impulse);
            // }
        }
        else if (value.phase == InputActionPhase.Canceled)
            disableStabilizer = false;
    }
    public void OnChangeShip(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
            SpaceShipManager.Instance.FocusNextShip();
    }


    public void OnPrimaryFire(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
        {
            PrimaryFire = true;
        }
        else if (value.phase == InputActionPhase.Canceled)
            PrimaryFire = false;
    }
    public void OnSecondaryFire(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Performed)
        {
            SecondaryFire = true;
        }
        else if (value.phase == InputActionPhase.Canceled)
            SecondaryFire = false;
    }




    private void FixedUpdate()
    {
        vRoll *= 0.991f;
        if (vForwardBackDoIncrement)
        {
            vForwardBack += vForwardBackIncrementor * 0.005f;
            vForwardBack = Mathf.Clamp(vForwardBack, -1, 1);
        }

        if (vLeftRightDoIncrement)
        {
            vLeftRight += vLeftRightIncrementor * 0.005f;
            vLeftRight = Mathf.Clamp(vLeftRight, -1, 1);
        }

        if (vUpDownDoIncrement)
        {
            vUpDown += vUpDownIncrementor * 0.005f;
            vUpDown = Mathf.Clamp(vUpDown, -1, 1);
        }

        if (vRollDoIncrement)
        {
            vRoll += vRollIncrementor * 0.01f;
            vRoll = Mathf.Clamp(vRoll, -1, 1);
        }
        // expectedVelocity = transform.TransformDirection(new Vector3(vLeftRight, vUpDown, vForwardBack));
        // expectedAngular = (transform.forward * vRoll);
        // expectedAngular += -transform.right * vAim.y;
        // expectedAngular += transform.up * vAim.x;
        // if (!disableStabilizer)
        // {
        //     if (expectedVelocity.sqrMagnitude <= 0.2f)
        //     {
        //         expectedVelocity = (-rb.velocity) * StabilizerCurve.Evaluate(rb.velocity.sqrMagnitude);
        //         // if (rb.velocity.sqrMagnitude > 1)
        //         //     expectedVelocity = (-rb.velocity).normalized * 10;
        //         // else
        //         //     expectedVelocity = (-rb.velocity).normalized;
        //     }
        //     else
        //     {
        //         if (Vector3.Dot(expectedVelocity, rb.velocity) < 0)
        //         {
        //             expectedVelocity = -rb.velocity + expectedVelocity;
        //         }
        //     }
        //     if (expectedAngular.sqrMagnitude <= 0.2f)
        //     {
        //         expectedAngular = (-rb.angularVelocity) * StabilizerCurve.Evaluate(rb.angularVelocity.sqrMagnitude);
        //         // if (rb.angularVelocity.sqrMagnitude > 0.5f)
        //         //     expectedAngular = (-rb.angularVelocity).normalized * 100;
        //         // else
        //         //     expectedAngular = (-rb.angularVelocity).normalized * rb.angularVelocity.sqrMagnitude;
        //     }
        //     else
        //     {
        //         if (Vector3.Dot(expectedAngular, rb.angularVelocity) < 0)
        //         {
        //             expectedAngular = -rb.angularVelocity + expectedAngular;
        //         }
        //     }
        // }
        // foreach (Thruster thruster in this.thrusters)
        // {
        //     float dotV = Vector3.Dot(thruster.transform.forward, expectedVelocity);
        //     if (dotV > 0.7f)
        //     {
        //         thruster.power = Mathf.InverseLerp(0.7f, 1, dotV);
        //     }
        //     else
        //     {
        //         thruster.power = 0;
        //     }

        //     float dotA = Vector3.Dot(Vector3.Cross(thruster.transform.position - rb.worldCenterOfMass, thruster.transform.forward), expectedAngular);
        //     // float dotA = Vector3.Dot(Vector3.Cross(thruster.transform.position - rb.worldCenterOfMass, thruster.transform.forward), expectedAngular);
        //     //thruster.power += Mathf.InverseLerp(-1, 1, dotA);
        //     if (dotA > 0.1f)
        //     {
        //         thruster.power += Mathf.InverseLerp(0.1f, 1, dotA);
        //     }
        // }
        // // // rb.AddRelativeForce(Vector3.forward*vForwardBack);
        // // // rb.AddRelativeForce(Vector3.right*vLeftRight);
        // // // rb.AddRelativeForce(Vector3.up*vUpDown);
        // // // rb.AddRelativeTorque(Vector3.forward*vRoll);
        // // // rb.AddRelativeTorque(0.005f*vAim.x*Vector3.up);
        // // // rb.AddRelativeTorque(0.005f*vAim.y*Vector3.left);
        // // vAim *= 0.999f;
    }
}
