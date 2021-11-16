using System;
using Unity.MLAgents;
using UnityEngine;

[ExecuteAlways]
public class MoodDisplay : MonoBehaviour
{
    [SerializeField] private GameObject leftSide;
    [SerializeField] private GameObject rightSide;
    [SerializeField] private float maxAngle = 60.0f;
    [SerializeField] private Agent parentAgent;

    public float mood = 0.0f;
    private void Awake()
    {
        parentAgent ??= GetComponentInParent<Agent>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        UpdateMood();
    }

    private void UpdateMood()
    {
        if (parentAgent is not null)
            mood = MathF.Tanh(parentAgent.GetCumulativeReward());
        float angle = Mathf.LerpUnclamped(0.0f, maxAngle, mood);
        leftSide.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -angle);
        rightSide.transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
    }
}
