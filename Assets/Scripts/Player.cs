using Fusion;
using UnityEngine;

namespace SubmarineStandoff
{
    public class Player : NetworkBehaviour
    {
        private const float _speed = 3f;
        
        [SerializeField] private NetworkTransform _networkTransform;

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                _networkTransform.transform.position += (Vector3) data.direction * (_speed * Runner.DeltaTime);
            }
        }
    }
}
