using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class SpaceshipAgent : Agent
{
    public Spaceship spaceship;
    public MLInputMgr MLInputMgr;
    public Transform Target;
    public Vector3 StartPosition;
    public Quaternion StartRotation;
    public Vector3 StartVelocity;
    public Vector3 StartAngularVelocity;
    public Vector3 EndPosition;
    public Quaternion EndRotation;
    public Vector3 EndVelocity;
    public Vector3 EndAngularVelocity;
    public LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnEpisodeBegin()
    {

        spaceship.rb.isKinematic = true;
        StartPosition = spaceship.rb.angularVelocity = Vector3.zero;// Random.insideUnitSphere * spaceship.MaxAngularVelocity;
        StartAngularVelocity = spaceship.rb.velocity = Vector3.zero;//Random.value * spaceship.transform.forward * spaceship.MaxForwardVelocity;

        StartPosition = Random.insideUnitSphere * 20;
        StartRotation = Random.rotation;
        spaceship.transform.SetPositionAndRotation(StartPosition + transform.position, StartRotation);
        // Move the target to a new spot
        EndPosition = Random.insideUnitSphere * 20;
        EndRotation = Random.rotation;

        Target.transform.SetPositionAndRotation(EndPosition + transform.position, EndRotation);
        spaceship.rb.isKinematic = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.position - spaceship.transform.position);
        // Agent velocity
        sensor.AddObservation(spaceship.rb.velocity);
        sensor.AddObservation(spaceship.rb.angularVelocity);
        //Agent Orientation
        float ForwardAngleToTarget = Vector3.Dot(spaceship.transform.forward, Target.forward);
        float RightAngleToTarget = Vector3.Dot(spaceship.transform.right, Target.right);
        sensor.AddObservation(spaceship.transform.rotation);
        sensor.AddObservation(ForwardAngleToTarget);
        sensor.AddObservation(RightAngleToTarget);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions
        Vector3 vAim = Vector2.zero;
        vAim.x = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        vAim.y = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
        vAim = Vector3.ClampMagnitude(vAim, 1);
        MLInputMgr.vAim = vAim;
        MLInputMgr.vForwardBack = Mathf.Clamp(actionBuffers.ContinuousActions[2], -1f, 1f);
        MLInputMgr.vLeftRight = Mathf.Clamp(actionBuffers.ContinuousActions[3], -1f, 1f);
        MLInputMgr.vRoll = Mathf.Clamp(actionBuffers.ContinuousActions[4], -1f, 1f);
        MLInputMgr.vUpDown = Mathf.Clamp(actionBuffers.ContinuousActions[5], -1f, 1f);
        MLInputMgr.disableStabilizer = Mathf.Clamp(actionBuffers.ContinuousActions[6], -1f, 1f) < 0;

        // Rewards
        float distanceToTarget = Vector3.Distance(spaceship.transform.localPosition, Target.localPosition);

        float ForwardAngleToTarget = Vector3.Dot(spaceship.transform.forward, Target.forward);
        float RightAngleToTarget = Vector3.Dot(spaceship.transform.right, Target.right);

        AddReward(-0.005f);
        // Reached target
        if (distanceToTarget < 3)
        {
            AddReward(10f);
            AddReward(2 * ForwardAngleToTarget);
            AddReward(2 * RightAngleToTarget);
            EndEpisode();
        }

        var m_MovingTowardsDot = Vector3.Dot(spaceship.rb.velocity, (Target.position - spaceship.transform.position).normalized);
        AddReward(0.01f * m_MovingTowardsDot);
        var m_PointingTowardsTarget = Vector3.Dot(spaceship.transform.forward, Target.forward);
        if (m_PointingTowardsTarget > 0 && m_MovingTowardsDot > 0)
            AddReward(0.01f * m_PointingTowardsTarget);

        // Out Of Safe Bounds
        if (distanceToTarget > 60 || spaceship.rb.angularVelocity.sqrMagnitude > (spaceship.MaxAngularVelocity * spaceship.MaxAngularVelocity) * 2)
        {
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = InputMgr.Instance.vAim.x;
        continuousActionsOut[1] = InputMgr.Instance.vAim.y;
        continuousActionsOut[2] = InputMgr.Instance.vForwardBack;
        continuousActionsOut[3] = InputMgr.Instance.vLeftRight;
        continuousActionsOut[4] = InputMgr.Instance.vRoll;
        continuousActionsOut[5] = InputMgr.Instance.vUpDown;
        continuousActionsOut[6] = InputMgr.Instance.disableStabilizer == true ? -1 : 1;
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, spaceship.transform.position);
        lineRenderer.SetPosition(1, Target.position);
    }
}
