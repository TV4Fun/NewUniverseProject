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

        [Tooltip("One-time reward given the first time the agent touches the target in an episode.")]
        public float collisionReward = 1f;

        [Tooltip("Reward given proportional to the impulse an agent applies to its target")]
        public float impulseReward = 0.01f;

        [Tooltip(
            "Reward given before the first physical contact with the target for getting closer to it (or punishment for getting further away).")]
        public float proximityReward = 0.01f;

        [Tooltip("Reward given on episode start.")]
        public float startingReward = 1f;

        [Tooltip("Time limit in seconds before episode automatically ends. Set to 0 to have no time limit.")]
        public float timeLimit = 0.0f;

        [Tooltip(
            "If unchecked, the episode will continue and the ball and agent will respawn after a fall. If checked, the episode will end and the full time penalty (if there is a limit) will be applied.")]
        public bool endEpisodeOnFall = true;
    }
}