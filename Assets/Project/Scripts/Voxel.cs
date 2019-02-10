using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    [RequireComponent(typeof(BoxCollider))]
    public class Voxel : OmiyaGames.PooledObject
    {
        [SerializeField]
        Vector2 range;
        [SerializeField]
        PooledObject dustParticles;

        public Vector3Int Coordinates
        {
            get;
            set;
        }

        public Vector2 Range
        {
            get
            {
                return range;
            }
        }

        public override void Awake()
        {
            base.Awake();

            // Bind to the deactivate event
            OnAfterDeactivated += Voxel_OnAfterDeactivated;
        }

        private void Voxel_OnAfterDeactivated(OmiyaGames.IPooledObject arg1, OmiyaGames.Global.PoolingManager arg2)
        {
            if(arg2 != null)
            {
                // Spawn the dust particles!
                PooledObject clone = arg2.GetInstance(dustParticles, transform.position, transform.rotation);
            }

            // Unbind to the deactivate event
            OnAfterDeactivated -= Voxel_OnAfterDeactivated;
        }
    }
}
