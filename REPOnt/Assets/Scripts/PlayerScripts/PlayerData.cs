using UnityEngine;

namespace PlayerScripts
{
    [CreateAssetMenu(fileName = "Data/PlayerData", menuName = "Data/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public float NormalSpeed;
        public float ReducedSpeed;
    }
}
