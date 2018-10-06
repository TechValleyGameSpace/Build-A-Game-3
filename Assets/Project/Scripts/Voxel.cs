using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class Voxel : OmiyaGames.IPooledObject
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
