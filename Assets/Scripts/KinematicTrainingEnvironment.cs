using System;
using System.Collections;
using DefaultNamespace;
using Unity.MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class KinematicTrainingEnvironment : MonoBehaviour
{
    [Serializable]
    private class TrackedObject
    {
        public TrackedObject(GameObject trackedObject, float fallThreshold)
        {
            rigidbody = trackedObject.GetComponent<Rigidbody>();
            transform = trackedObject.GetComponent<Transform>();
            startingHeight = transform.localPosition.y;
            m_MinHeight = startingHeight - fallThreshold;
        }

        public bool HasJustFallen()
        {
            if (!m_HasFallen && transform.localPosition.y < m_MinHeight)
            {
                m_HasFallen = true;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            transform.localPosition = new Vector3(Random.Range(-5.5f, 5.5f), startingHeight, Random.Range(-4.4f, 4.4f));
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            m_HasFallen = false;
        }

        public readonly Rigidbody rigidbody;
        public readonly Transform transform;
        public readonly float startingHeight;
        private bool m_HasFallen = false;
        private readonly float m_MinHeight;
    }

    [SerializeField] private Agent agent;

    [SerializeField] private GameObject target;

    public RewardParameters rewardParameters;

    [SerializeField] private MeshRenderer floor;

    [SerializeField] private Material winMaterial;

    [SerializeField] private Material loseMaterial;

    [SerializeField] private float fallThreshold = 0.2f;

    private TrackedObject m_Target;
    private TrackedObject m_Agent;
    private float m_TargetDistance;
    private bool m_HaveTouched = false;
    private float m_ElapsedTime;

    private void Awake()
    {
        m_Target = new TrackedObject(target, fallThreshold);
        m_Agent = new TrackedObject(agent.gameObject, fallThreshold);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        AddProximityReward();
        CheckForFall();
        CheckForWin();
        CheckTime();
    }

    private void AddProximityReward()
    {
        if (!m_HaveTouched) // We don't want to disincentivize pushing the ball away.
        {
            float newDistance = Vector3.Distance(m_Agent.transform.position, m_Target.transform.position);
            agent.AddReward(rewardParameters.proximityReward * (m_TargetDistance - newDistance));
            m_TargetDistance = newDistance;
        }
    }

    private void CheckForFall()
    {
        if (m_Agent.HasJustFallen())
        {
            agent.AddReward(-rewardParameters.deathPenalty);
            floor.material = loseMaterial;
            if (rewardParameters.endEpisodeOnFall)
            {
                if (rewardParameters.timeLimit > 0.0f)
                {
                    // No cheating by jumping over to spare running out the clock.
                    agent.AddReward(-rewardParameters.timePenalty * (rewardParameters.timeLimit - m_ElapsedTime));
                }

                StartCoroutine(EndEpisodeAfterDelay(1.0f));
            }
            else
            {
                Respawn();
            }
        }
    }

    private void CheckForWin()
    {
        if (m_Target.HasJustFallen())
        {
            agent.AddReward(rewardParameters.winReward);
            floor.material = winMaterial;
            StartCoroutine(EndEpisodeAfterDelay(1.0f));
        }
    }

    private void CheckTime()
    {
        agent.AddReward(-rewardParameters.timePenalty * Time.deltaTime);
        m_ElapsedTime += Time.deltaTime;
        if (rewardParameters.timeLimit > 0.0 && m_ElapsedTime >= rewardParameters.timeLimit)
        {
            agent.EndEpisode();
        }
    }

    public void NewEpisode()
    {
        m_ElapsedTime = 0.0f;
        StopAllCoroutines(); // Stop any delayed episode ends that might be happening.
        Respawn();
        agent.SetReward(rewardParameters.startingReward);
    }

    public void NotifyTouch()
    {
        m_HaveTouched = true;
        agent.AddReward(rewardParameters.collisionReward);
    }

    private void Respawn()
    {
        do
        {
            m_Target.Reset();
            m_Agent.Reset();
        } while ((m_Agent.transform.localPosition - m_Target.transform.localPosition).sqrMagnitude < 1.5f);

        m_Agent.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        m_TargetDistance = Vector3.Distance(m_Agent.transform.position, m_Target.transform.position);
        m_HaveTouched = false;
    }

    private IEnumerator EndEpisodeAfterDelay(float delay = 1.0f)
    {
        yield return new WaitForSeconds(delay);
        agent.EndEpisode();
    }
}