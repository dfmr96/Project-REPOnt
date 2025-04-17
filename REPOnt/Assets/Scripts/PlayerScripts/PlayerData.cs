using UnityEngine;

namespace PlayerScripts
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data")]
    public class PlayerData : ScriptableObject
    {
        public float NormalSpeed;
        public float ReducedSpeed;
    }
}
