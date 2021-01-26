using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MathNet.Numerics.LinearAlgebra;

public class Spaceship : MonoBehaviour
{
    public Rigidbody rb;
    public List<Thruster> thrusters;
    public List<Thruster> mainThrusters;
    public List<Thruster> reverseThrusters;
    public List<Thruster> leftlateralThrusters;
    public List<Thruster> rightlateralThrusters;
    private Matrix<double> ThrustVectorsMatrix;
    public List<Thruster> dorsalThrusters;
    public List<Thruster> ventralThrusters;

    public Transform CameraPosition;
    public Transform CoM;

    public IInputMgr inputMgr;
    public Vector3 maxForce = Vector3.zero;
    public Vector3 minForce = Vector3.zero;
    public Vector3 maxTorque = Vector3.zero;
    public Vector3 minTorque = Vector3.zero;


    public float MaxForwardVelocity = 220;
    public float MaxRightVelocity = 10;
    public float MaxUpVelocity = 20;
    public float ForwardThrottle = 0;
    public float MaxVelocity = 330;
    public float MaxAngularVelocity = 5;

    public Transform WeaponTarget;
    public List<Weapon> Weapons;

    // Start is called before the first frame update
    void Start()
    {
        BootUpThrusters();
        CoM.localPosition = rb.centerOfMass;
        rb.maxAngularVelocity = 100;
        Weapons = GetComponentsInChildren<Weapon>().ToList();
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

        ThrustVectorsMatrix = Matrix<double>.Build.Dense(6, thrusters.Count);

        var thrusterIndex = 0;
        foreach (Thruster thruster in this.thrusters)
        {
            thruster.InitializeThrustVector();
            ThrustVectorsMatrix.SetColumn(thrusterIndex,
             new double[]{
                thruster.TorqueVector.x,
                thruster.TorqueVector.y,
                thruster.TorqueVector.z,
                thruster.ForceVector.x,
                thruster.ForceVector.y,
                thruster.ForceVector.z,
            });
            thrusterIndex++;
        }
        ThrustVectorsMatrix = ThrustVectorsMatrix.NormalizeRows(1);
        thrusterControlVector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(thrusters.Count, 0);
        minControlVector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(thrusters.Count, 0);
        maxControlVector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(thrusters.Count, 1);
        initialGuessControlVector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(thrusters.Count, 0);
        double[,] twelveDirectionArray = {//12 = 6 Components (Tx,Ty,Tz,Fx,Fy,Fz) in 2 (+,-) directions
                      //{Tx,Ty,Tz,Fx,Fy,Fz}
                        { 1, 0, 0, 0, 0, 0},
                        {-1, 0, 0, 0, 0, 0},
                        { 0, 1, 0, 0, 0, 0},
                        { 0,-1, 0, 0, 0, 0},
                        { 0, 0, 1, 0, 0, 0},
                        { 0, 0,-1, 0, 0, 0},
                        { 0, 0, 0, 1, 0, 0},
                        { 0, 0, 0,-1, 0, 0},
                        { 0, 0, 0, 0, 1, 0},
                        { 0, 0, 0, 0,-1, 0},
                        { 0, 0, 0, 0, 0, 1},
                        { 0, 0, 0, 0, 0,-1},
                    };
        twelveInputVectors = Matrix<double>.Build.DenseOfArray(twelveDirectionArray);
        twelveControlVectors = Matrix<double>.Build.Dense(12, thrusters.Count);
        foreach (var row in twelveInputVectors.EnumerateRowsIndexed())
        {
            twelveControlVectors.SetRow(row.Item1, MathNet.Numerics.FindMinimum.OfFunctionConstrained(guessedControlVector =>
                             {
                                 return ThrustVectorsMatrix.Multiply(guessedControlVector)
                                 .Subtract(row.Item2)
                                 .PointwisePower(2).Sum();
                             },
                            minControlVector,//LowerBound
                            maxControlVector,//UpperBound
                            initialGuessControlVector //InitialGuess
                    ));
        }
        // Debug.Log(ThrustVectorsMatrix.ToString(6, thrusters.Count));//6 Components
        // Debug.Log(twelveControlVectors.ToString(12, thrusters.Count));//6 Components in 2 directions
    }



    MathNet.Numerics.LinearAlgebra.Vector<double> UserInputVector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(6);//6 Components (Tx,Ty,Tz,Fx,Fy,Fz)
    private Vector<double> thrusterControlVector;
    private Vector<double> minControlVector;
    private Vector<double> maxControlVector;
    private Vector<double> initialGuessControlVector;
    private Matrix<double> twelveInputVectors;
    private Matrix<double> twelveControlVectors;

    private void FixedUpdate()
    {
        var UserExpectedForwardVelocity = inputMgr.vForwardBack * MaxForwardVelocity;
        var UserExpectedRightVelocity = inputMgr.vLeftRight * MaxRightVelocity;
        var UserExpectedUpVelocity = inputMgr.vUpDown * MaxUpVelocity;
        var UserExpectedVelocity = new Vector3(UserExpectedRightVelocity, UserExpectedUpVelocity, UserExpectedForwardVelocity);
        var UserExpectedAngularVelocity = (Vector3.forward * inputMgr.vRoll);

        //var userInputVelocity = new Vector3(inputMgr.vLeftRight, inputMgr.vUpDown, inputMgr.vForwardBack);
        //var userInputAngularVelocity = (Vector3.forward * inputMgr.vRoll);
        if (inputMgr.vAim.sqrMagnitude > 0.02f)//Deadzone
        {
            UserExpectedAngularVelocity += -Vector3.right * inputMgr.vAim.y * InputMgr.Instance.AimSensitivityCurve.Evaluate(inputMgr.vAim.sqrMagnitude);
            UserExpectedAngularVelocity += Vector3.up * inputMgr.vAim.x * InputMgr.Instance.AimSensitivityCurve.Evaluate(inputMgr.vAim.sqrMagnitude);
        }
        var stabilize = !inputMgr.disableStabilizer;
        if (stabilize)
        {
            var currentVelocity = transform.InverseTransformDirection(rb.velocity);
            var currentAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);

            var deltaVelocity = UserExpectedVelocity - currentVelocity;
            var deltaAngularVelocity = UserExpectedAngularVelocity - currentAngularVelocity;
            UserExpectedVelocity.x = Mathf.Clamp(deltaVelocity.x, -MaxRightVelocity, MaxRightVelocity);
            UserExpectedVelocity.y = Mathf.Clamp(deltaVelocity.y, -MaxUpVelocity, MaxUpVelocity);
            UserExpectedVelocity.z = Mathf.Clamp(deltaVelocity.z, -MaxForwardVelocity, MaxForwardVelocity);
            UserExpectedVelocity = Vector3.ClampMagnitude(UserExpectedVelocity, MaxVelocity);
            UserExpectedAngularVelocity = Vector3.ClampMagnitude(deltaAngularVelocity, MaxAngularVelocity) * 10;
            // if (Vector3.Dot(userInputVelocity, currentVelocity.normalized) <= .5f)
            // {
            //     userInputVelocity = (userInputVelocity - (currentVelocity * .5f));
            //     if (userInputVelocity.sqrMagnitude > 0 && userInputVelocity.sqrMagnitude <= 1)//Speed up braking when slow
            //         userInputVelocity = userInputVelocity.normalized * InputMgr.StabilizerCurve.Evaluate(userInputVelocity.sqrMagnitude);
            // }
            // if (Vector3.Dot(userInputAngularVelocity, currentAngularVelocity.normalized) <= 1f)
            // {
            //     userInputAngularVelocity = (userInputAngularVelocity - (currentAngularVelocity * 1f));
            //     if (userInputAngularVelocity.sqrMagnitude > 0 && userInputAngularVelocity.sqrMagnitude <= 1)//Speed up braking when slow
            //         userInputAngularVelocity = userInputAngularVelocity.normalized * InputMgr.StabilizerCurve.Evaluate(userInputAngularVelocity.sqrMagnitude);
            // }
        }

        UserInputVector[0] = UserExpectedAngularVelocity.x;
        UserInputVector[1] = UserExpectedAngularVelocity.y;
        UserInputVector[2] = UserExpectedAngularVelocity.z;
        UserInputVector[3] = UserExpectedVelocity.x;
        UserInputVector[4] = UserExpectedVelocity.y;
        UserInputVector[5] = UserExpectedVelocity.z;

        thrusterControlVector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(thrusters.Count);
        foreach (var component in UserInputVector.EnumerateIndexed())
        {
            if (component.Item2 >= 0)
            {
                thrusterControlVector = thrusterControlVector.Add(twelveControlVectors.Row(2 * component.Item1) * component.Item2);//Use positive Torque or Force ControlVector
            }
            else
            {
                thrusterControlVector = thrusterControlVector.Add(twelveControlVectors.Row((2 * component.Item1) + 1) * -component.Item2);//Use negative Torque or Force ControlVector, also negate component since its negative
            }
        }
        var AbsoluteMaximum = thrusterControlVector.AbsoluteMaximum();
        if (AbsoluteMaximum > 1)
            thrusterControlVector = thrusterControlVector.Divide(AbsoluteMaximum);
        //Debug.Log("UserInputVector=" + UserInputVector);
        for (int i = 0; i < thrusters.Count; i++)
        {
            thrusters[i].power = (float)thrusterControlVector[i];
        }
    }

    // void Update()
    // {
    //     if (Physics.Raycast(transform.position, transform.forward, out var hitinfo, 1000))
    //     {
    //         WeaponTarget.position = hitinfo.point;
    //     }
    //     else
    //     {
    //         WeaponTarget.position = transform.position + transform.forward * 10;
    //     }
    // }
}
