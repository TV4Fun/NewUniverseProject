using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class KinematicAgent : Agent
{
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float turnSpeed = 1.0f;

    private Vector3 m_ContactForce;
    private Vector3 m_ContactPoint;
    private float m_ElapsedTime;

    private Rigidbody m_SelfBody;
    private KinematicTrainingEnvironment m_Environment;

    public override void Initialize()
    {
        m_SelfBody = GetComponent<Rigidbody>();
        m_Environment = GetComponentInParent<KinematicTrainingEnvironment>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            m_Environment.NotifyTouch();
            // Debug.Log("Collided with target");
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            m_ContactPoint = Vector3.zero;
            m_ContactForce = Vector3.zero;
        }
        // Debug.Log("Stop colliding with target");
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Target"))
            ProcessTouch(other);
        // Debug.Log("Still colliding with target");
    }

    public override void OnEpisodeBegin()
    {
        m_Environment.NewEpisode();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 move = new Vector3(actions.ContinuousActions[0], 0f, actions.ContinuousActions[1]) *
                       (Time.deltaTime * moveSpeed);
        AddReward(-m_Environment.rewardParameters.movePenalty * move.sqrMagnitude * Time.deltaTime);
        float turn = actions.ContinuousActions[2] * Time.deltaTime * turnSpeed;
        AddReward(-m_Environment.rewardParameters.turnPenalty * turn * turn * Time.deltaTime);
        //Debug.Log(move);
        //Debug.Log(turn);

        m_SelfBody.AddRelativeForce(move);
        m_SelfBody.AddRelativeTorque(0f, turn, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition / 10.0f);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(m_ContactPoint);
        sensor.AddObservation(m_ContactForce);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Just use the controller for movement
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Strafe");
        continuousActions[1] = Input.GetAxisRaw("Forward");
        continuousActions[2] = Input.GetAxisRaw("Rotate");
    }

    private void ProcessTouch(Collision other)
    {
        // Provide information on force and point of contact.
        Vector3 impulse = other.impulse;
        AddReward(m_Environment.rewardParameters.impulseReward * impulse.magnitude);
        m_ContactForce = transform.InverseTransformDirection(impulse / Time.fixedDeltaTime);
        Vector3 contactPoint = other.GetContact(0).point;
        // Use Average if there is more than one contact point.
        for (int i = 1; i < other.contactCount; ++i) contactPoint += other.GetContact(i).point;

        m_ContactPoint = transform.InverseTransformPoint(contactPoint / other.contactCount);
    }
}