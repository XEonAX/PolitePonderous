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
    public float vUpDown;
    public float vRoll;
    public Vector2 vAim;
    public bool disableStabilizer = false;
    public AnimationCurve StabilizerCurve;
    public AnimationCurve AimSensitivityCurve;
    public Vector3 expectedVelocity;
    public Vector3 expectedAngular;
    public static InputMgr Instance;
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
        vLeftRight = value.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext value)
    {
        vUpDown = value.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext value)
    {
        vRoll = value.ReadValue<float>();
    }
    public void OnAim(InputAction.CallbackContext value)
    {
        var vAimDelta = value.ReadValue<Vector2>();
        //Debug.Log(vAim.sqrMagnitude + "  " + vAimDelta);
        vAim += vAimDelta * AimSensitivityCurve.Evaluate(vAim.sqrMagnitude);
        vAim = Vector3.ClampMagnitude(vAim, 1);
    }
    public void OnResetForwardBack(InputAction.CallbackContext value)
    {
        vForwardBack = 0;
        vForwardBackIncrementor = 0;
        vAim = Vector2.zero;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    private void FixedUpdate()
    {
        if (vForwardBackDoIncrement)
        {
            vForwardBack += vForwardBackIncrementor * 0.25f;
            vForwardBack = Mathf.Clamp(vForwardBack, -1, 1);
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
