using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(BoxCollider))]
    public class Voxel : OmiyaGames.PooledObject
    {
        [SerializeField]
        Vector2 range;

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
    }
}
