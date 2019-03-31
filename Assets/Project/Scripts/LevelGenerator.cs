using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    public class LevelGenerator : MonoBehaviour
    {
        const int StartRangeLimit = int.MaxValue / 10;

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
        StoryHistory history;

        [Header("Initial Setup")]
        [SerializeField]
        Gradient upperVoxelColors;
        [SerializeField]
        Gradient lowerVoxelColors;
        [SerializeField]
        [Range(0.01f, 0.5f)]
        float colorNoiseCoordinate = 1f;
        
        [Header("Collectable")]
        [SerializeField]
        [Range(0f, 0.5f)]
        float collectableProbability = 0.1f;
        [SerializeField]
        Collectable[] allCollectables;

        Vector3Int initialPosition = new Vector3Int();
        Vector3Int colorPosition = new Vector3Int();
        Dictionary<Vector3Int, Voxel> createdVoxels = null;
        Dictionary<Vector3Int, Collectable> createdCollectables = null;
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

        public Dictionary<Vector3Int, Collectable> CreatedCollectables
        {
            get
            {
                if (createdCollectables == null)
                {
                    createdCollectables = new Dictionary<Vector3Int, Collectable>((int)Mathf.Pow(radiusFromPlayer * 2, 3));
                }
                return createdCollectables;
            }
        }

        // Use this for initialization
        void Start()
        {
            initialPosition.x = Random.Range(-StartRangeLimit, StartRangeLimit);
            initialPosition.y = Random.Range(-StartRangeLimit, StartRangeLimit);
            initialPosition.z = Random.Range(-StartRangeLimit, StartRangeLimit);
            colorPosition.x = Random.Range(-StartRangeLimit, StartRangeLimit);
            colorPosition.y = Random.Range(-StartRangeLimit, StartRangeLimit);
            colorPosition.z = Random.Range(-StartRangeLimit, StartRangeLimit);
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
            AdjustYPlane();
            AdjustZPlane();
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

        public IEnumerable<Vector3Int> NextCoordinatesY(Vector3Int fromPosition, params int[] ys)
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

        public IEnumerable<Vector3Int> NextCoordinatesZ(Vector3Int fromPosition, params int[] zs)
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
            if(pos.sqrMagnitude <= 4)
            {
                return null;
            }

            // Calculate position
            Vector3 offsetCoordinate = pos - initialPosition;
            offsetCoordinate *= noiseCoordinate;

            // Grab a voxel
            float perlinNoise = Perlin.Noise(offsetCoordinate);
            Voxel instance = null, prefab = GetVoxel(perlinNoise);
            if (prefab != null)
            {
                // Calculate color noise
                offsetCoordinate = pos - colorPosition;
                offsetCoordinate *= colorNoiseCoordinate;
                perlinNoise = Perlin.Noise(offsetCoordinate);
                perlinNoise = Mathf.Repeat(perlinNoise, 1);

                // Grab an instance of the voxel
                instance = Singleton.Get<PoolingManager>().GetInstance<Voxel>(prefab);
                instance.VoxelColor = Color.Lerp(lowerVoxelColors.Evaluate(perlinNoise), upperVoxelColors.Evaluate(perlinNoise), Random.value);
                CreatedVoxels.Add(pos, instance);

                // Position the voxel
                instance.transform.position = pos;
            }

            if(Random.value < collectableProbability)
            {
                // Grab an instance of a collectable
                Collectable randomCollectablePrefab = allCollectables[Random.Range(0, allCollectables.Length)];
                Collectable collectableInstance = Singleton.Get<PoolingManager>().GetInstance<Collectable>(randomCollectablePrefab);
                collectableInstance.Logs = history;
                CreatedCollectables.Add(pos, collectableInstance);

                // Position the voxel
                collectableInstance.transform.position = pos;
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

        private void AdjustYPlane()
        {
            if (currentPlayerPosition.y > lastPlayerPosition.y)
            {
                AdjustYPlane((lastPlayerPosition.y - radiusFromPlayer), (lastPlayerPosition.y + radiusFromPlayer + 1));
                lastPlayerPosition.y += 1;
            }
            else if (currentPlayerPosition.y < lastPlayerPosition.y)
            {
                AdjustYPlane((lastPlayerPosition.y + radiusFromPlayer), (lastPlayerPosition.y - radiusFromPlayer - 1));
                lastPlayerPosition.y -= 1;
            }
        }

        private void AdjustZPlane()
        {
            if (currentPlayerPosition.z > lastPlayerPosition.z)
            {
                AdjustZPlane((lastPlayerPosition.z - radiusFromPlayer), (lastPlayerPosition.z + radiusFromPlayer + 1));
                lastPlayerPosition.z += 1;
            }
            else if (currentPlayerPosition.z < lastPlayerPosition.z)
            {
                AdjustZPlane((lastPlayerPosition.z + radiusFromPlayer), (lastPlayerPosition.z - radiusFromPlayer - 1));
                lastPlayerPosition.z -= 1;
            }
        }

        private void AdjustXPlane(int remove, int add)
        {
            AdjustPlane(NextCoordinatesX(lastPlayerPosition, remove, add));
        }
        private void AdjustYPlane(int remove, int add)
        {
            AdjustPlane(NextCoordinatesY(lastPlayerPosition, remove, add));
        }
        private void AdjustZPlane(int remove, int add)
        {
            AdjustPlane(NextCoordinatesZ(lastPlayerPosition, remove, add));
        }

        private void AdjustPlane(IEnumerable<Vector3Int> planeDirection)
        {
            bool toAdd = false;
            foreach (Vector3Int nextCoordinate in planeDirection)
            {
                // Remove voxels in the negative X-direction
                if (toAdd == false)
                {
                    if (CreatedVoxels.ContainsKey(nextCoordinate) == true)
                    {
                        PoolingManager.ReturnToPool(CreatedVoxels[nextCoordinate]);
                        CreatedVoxels.Remove(nextCoordinate);
                    }
                    if(CreatedCollectables.ContainsKey(nextCoordinate)== true)
                    {
                        PoolingManager.ReturnToPool(CreatedCollectables[nextCoordinate]);
                        CreatedCollectables.Remove(nextCoordinate);
                    }
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
            // Return a random voxel
            return allVoxels[Random.Range(0, allVoxels.Length)];
            //// Setup the cache list
            //if (getVoxelCache == null)
            //{
            //    getVoxelCache = new List<Voxel>(allVoxels.Length);
            //}
            //getVoxelCache.Clear();

            //// Go through all voxels
            //foreach (Voxel consider in allVoxels)
            //{
            //    // Check if value is in-between
            //    if ((value > consider.Range.x) && (value < consider.Range.y))
            //    {
            //        getVoxelCache.Add(consider);
            //    }
            //}

            //// Return a random voxel
            //Voxel returnVoxel = null;
            //if (getVoxelCache.Count > 0)
            //{
            //    returnVoxel = getVoxelCache[0];
            //    if (getVoxelCache.Count > 1)
            //    {
            //        returnVoxel = getVoxelCache[Random.Range(0, getVoxelCache.Count)];
            //    }
            //}
            //return returnVoxel;
        }
    }
}
