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
        [SerializeField]
        Renderer changeColor;

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

        public Color VoxelColor
        {
            set => changeColor.material.SetColor("_BaseColor", value);
        }

        public void ExplodeVoxel()
        {
            // Spawn the dust particles!
            PooledObject clone = Singleton.Get<PoolingManager>().GetInstance(dustParticles, transform.position, transform.rotation);
            OmiyaGames.Global.PoolingManager.ReturnToPool(this);
        }
    }
}
