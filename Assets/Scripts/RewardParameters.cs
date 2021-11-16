using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Reward", menuName = "Reward Parameters", order = 0)]
    public class RewardParameters : ScriptableObject
    {
        public float deathPenalty = 1f;
        public float timePenalty = 0.01f;
        public float movePenalty = 0.02f;
        public float turnPenalty = 0.02f;
        public float winReward = 1f;
        public float collisionReward = 1f;
        public float impulseReward = 0.01f;
        public float proximityReward = 0.01f;
        public float startingReward = 1f;
        [Tooltip("Time limit in seconds before episode automatically ends. Set to 0 to have no time limit.")]
        public float timeLimit = 0.0f;
        [Tooltip("If unchecked, the episode will continue and the ball and agent will respawn after a fall. If checked, the episode will end and the full time penalty (if there is a limit) will be applied.")]
        public bool endEpisodeOnFall = true;
    }
}