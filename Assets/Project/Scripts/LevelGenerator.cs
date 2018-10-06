using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField]
        Voxel[] allVoxels;
        [SerializeField]
        int radiusFromPlayer = 10;
        [SerializeField]
        [Range(0.01f, 0.5f)]
        float noiseCoordinate = 1f;

        [Header("Initial Setup")]
        [SerializeField]
        Transform player;
        [SerializeField]
        Vector3Int initialPosition = new Vector3Int();

        List<Voxel> createdVoxels = null;
        List<Voxel> getVoxelCache = null;

        public List<Voxel> CreatedVoxels
        {
            get
            {
                if(createdVoxels == null)
                {
                    createdVoxels = new List<Voxel>((int)Mathf.Pow(radiusFromPlayer * 2, 3));
                }
                return createdVoxels;
            }
        }

        // Use this for initialization
        void Start()
        {
            Voxel prefab, instance;
            Vector3 location;
            foreach(Vector3Int pos in SurroundingCoordinates(Vector3Int.zero))
            {
                // Grab a voxel
                prefab = GetVoxel(NoiseValue(pos));
                if (prefab != null)
                {
                    // Update location
                    location.x = pos.x;
                    location.y = pos.y;
                    location.z = pos.z;

                    // Grab an instance of the voxel
                    instance = Singleton.Get<PoolingManager>().GetInstance<Voxel>(prefab);
                    CreatedVoxels.Add(instance);

                    // Position the voxel
                    instance.transform.position = location;
                }
            }
        }

        private float NoiseValue(Vector3Int fromPosition)
        {
            Vector3 offsetCoordinate = fromPosition - initialPosition;
            offsetCoordinate *= noiseCoordinate;
            return Perlin.Noise(offsetCoordinate);
        }

        public IEnumerable<Vector3Int> SurroundingCoordinates(Vector3Int fromPosition)
        {
            Vector3Int nextCoordinate = fromPosition;
            for (int x = (fromPosition.x - radiusFromPlayer); x <= (fromPosition.x + radiusFromPlayer); ++x)
            {
                for (int y = (fromPosition.y - radiusFromPlayer); y <= (fromPosition.y + radiusFromPlayer); ++y)
                {
                    for (int z = (fromPosition.z - radiusFromPlayer); z <= (fromPosition.z - radiusFromPlayer); ++z)
                    {
                        nextCoordinate.x = x;
                        nextCoordinate.y = y;
                        nextCoordinate.z = z;
                        yield return nextCoordinate;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // FIXME: remove voxels based on distance, then create new ones.
        }

        public Voxel GetVoxel(float value)
        {
            // Setup the cache list
            if(getVoxelCache == null)
            {
                getVoxelCache = new List<Voxel>(allVoxels.Length);
            }
            getVoxelCache.Clear();

            // Go through all voxels
            foreach (Voxel consider in allVoxels)
            {
                // Check if value is in-between
                if((value > consider.Range.x) && (value < consider.Range.y))
                {
                    getVoxelCache.Add(consider);
                }
            }

            // Return a random voxel
            Voxel returnVoxel = null;
            if(getVoxelCache.Count > 0)
            {
                returnVoxel = getVoxelCache[0];
                if(getVoxelCache.Count > 1)
                {
                    returnVoxel = getVoxelCache[Random.Range(0, getVoxelCache.Count)];
                }
            }
            return returnVoxel;
        }
    }
}
