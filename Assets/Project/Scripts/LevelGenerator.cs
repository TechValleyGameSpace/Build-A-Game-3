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

        Dictionary<Vector3Int, Voxel> createdVoxels = null;
        List<Voxel> getVoxelCache = null;
        Vector3 playerPosition;
        Vector3Int currentPlayerPosition = Vector3Int.zero;
        Vector3Int lastPlayerPosition = Vector3Int.zero;

        public Dictionary<Vector3Int, Voxel> CreatedVoxels
        {
            get
            {
                if (createdVoxels == null)
                {
                    createdVoxels = new Dictionary<Vector3Int, Voxel>((int)Mathf.Pow(radiusFromPlayer * 2, 3));
                }
                return createdVoxels;
            }
        }

        // Use this for initialization
        void Start()
        {
            foreach (Vector3Int pos in SurroundingCoordinates(Vector3Int.zero))
            {
                AddVoxel(pos);
            }

            // Calculate the player position
            SetPosition(player.position, ref lastPlayerPosition);
        }

        // Update is called once per frame
        void Update()
        {
            // FIXME: remove voxels based on distance, then create new ones.
            SetPosition(player.position, ref currentPlayerPosition);
            AdjustXPlane();
        }

        public IEnumerable<Vector3Int> SurroundingCoordinates(Vector3Int fromPosition)
        {
            Vector3Int nextCoordinate = fromPosition;
            for (int x = (fromPosition.x - radiusFromPlayer); x <= (fromPosition.x + radiusFromPlayer); ++x)
            {
                for (int y = (fromPosition.y - radiusFromPlayer); y <= (fromPosition.y + radiusFromPlayer); ++y)
                {
                    for (int z = (fromPosition.z - radiusFromPlayer); z <= (fromPosition.z + radiusFromPlayer); ++z)
                    {
                        nextCoordinate.x = x;
                        nextCoordinate.y = y;
                        nextCoordinate.z = z;
                        yield return nextCoordinate;
                    }
                }
            }
        }

        public IEnumerable<Vector3Int> NextCoordinatesX(Vector3Int fromPosition, params int[] xs)
        {
            Vector3Int nextCoordinate = fromPosition;
            for (int y = (fromPosition.y - radiusFromPlayer); y <= (fromPosition.y + radiusFromPlayer); ++y)
            {
                for (int z = (fromPosition.z - radiusFromPlayer); z <= (fromPosition.z + radiusFromPlayer); ++z)
                {
                    foreach(int x in xs)
                    {
                        nextCoordinate.x = x;
                        nextCoordinate.y = y;
                        nextCoordinate.z = z;
                        yield return nextCoordinate;
                    }
                }
            }
        }

        public IEnumerable<Vector3Int> SurroundingCoordinatesY(Vector3Int fromPosition, params int[] ys)
        {
            Vector3Int nextCoordinate = fromPosition;
            for (int x = (fromPosition.x - radiusFromPlayer); x <= (fromPosition.x + radiusFromPlayer); ++x)
            {
                for (int z = (fromPosition.z - radiusFromPlayer); z <= (fromPosition.z + radiusFromPlayer); ++z)
                {
                    foreach (int y in ys)
                    {
                        nextCoordinate.x = x;
                        nextCoordinate.y = y;
                        nextCoordinate.z = z;
                        yield return nextCoordinate;
                    }
                }
            }
        }

        public IEnumerable<Vector3Int> SurroundingCoordinatesZ(Vector3Int fromPosition, params int[] zs)
        {
            Vector3Int nextCoordinate = fromPosition;
            for (int x = (fromPosition.x - radiusFromPlayer); x <= (fromPosition.x + radiusFromPlayer); ++x)
            {
                for (int y = (fromPosition.y - radiusFromPlayer); y <= (fromPosition.y + radiusFromPlayer); ++y)
                {
                    foreach (int z in zs)
                    {
                        nextCoordinate.x = x;
                        nextCoordinate.y = y;
                        nextCoordinate.z = z;
                        yield return nextCoordinate;
                    }
                }
            }
        }

        private Voxel AddVoxel(Vector3Int pos)
        {
            // Calculate position
            Vector3 offsetCoordinate = pos - initialPosition;
            offsetCoordinate *= noiseCoordinate;

            // Grab a voxel
            Voxel instance = null, prefab = GetVoxel(Perlin.Noise(offsetCoordinate));
            if (prefab != null)
            {
                // Grab an instance of the voxel
                instance = Singleton.Get<PoolingManager>().GetInstance<Voxel>(prefab);
                CreatedVoxels.Add(pos, instance);

                // Position the voxel
                instance.transform.position = pos;
            }
            return instance;
        }

        private static void SetPosition(Vector3 pos, ref Vector3Int result)
        {
            result.x = Mathf.FloorToInt(pos.x);
            result.y = Mathf.FloorToInt(pos.y);
            result.z = Mathf.FloorToInt(pos.z);
        }

        private void AdjustXPlane()
        {
            if (currentPlayerPosition.x > lastPlayerPosition.x)
            {
                AdjustXPlane((lastPlayerPosition.x - radiusFromPlayer), (lastPlayerPosition.x + radiusFromPlayer + 1));
                lastPlayerPosition.x += 1;
            }
            else if (currentPlayerPosition.x < lastPlayerPosition.x)
            {
                AdjustXPlane((lastPlayerPosition.x + radiusFromPlayer), (lastPlayerPosition.x - radiusFromPlayer - 1));
                lastPlayerPosition.x -= 1;
            }
        }

        private void AdjustXPlane(int removeX, int addX)
        {
            bool toAdd = false;
            foreach (Vector3Int nextCoordinate in NextCoordinatesX(lastPlayerPosition, removeX, addX))
            {
                // Remove voxels in the negative X-direction
                if ((toAdd == false) && (CreatedVoxels.ContainsKey(nextCoordinate) == true))
                {
                    PoolingManager.ReturnToPool(CreatedVoxels[nextCoordinate]);
                    CreatedVoxels.Remove(nextCoordinate);
                }
                else if (toAdd == true)
                {
                    // Add voxels in the positive X-direction
                    AddVoxel(nextCoordinate);
                }
                toAdd = !toAdd;
            }
        }

        public Voxel GetVoxel(float value)
        {
            // Setup the cache list
            if (getVoxelCache == null)
            {
                getVoxelCache = new List<Voxel>(allVoxels.Length);
            }
            getVoxelCache.Clear();

            // Go through all voxels
            foreach (Voxel consider in allVoxels)
            {
                // Check if value is in-between
                if ((value > consider.Range.x) && (value < consider.Range.y))
                {
                    getVoxelCache.Add(consider);
                }
            }

            // Return a random voxel
            Voxel returnVoxel = null;
            if (getVoxelCache.Count > 0)
            {
                returnVoxel = getVoxelCache[0];
                if (getVoxelCache.Count > 1)
                {
                    returnVoxel = getVoxelCache[Random.Range(0, getVoxelCache.Count)];
                }
            }
            return returnVoxel;
        }
    }
}
