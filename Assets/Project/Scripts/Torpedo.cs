using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(Rigidbody))]
    public class Torpedo : OmiyaGames.PooledObject
    {
        [SerializeField]
        float destroyAfterSeconds = 10f;

        Coroutine delayDestroyCoroutine;

        public override void Start()
        {
            base.Start();
            delayDestroyCoroutine = StartCoroutine(DelayDestroy());
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Destructable") == true)
            {
                Voxel voxel = collision.collider.GetComponent<Voxel>();
                if(voxel != null)
                {
                    OmiyaGames.Global.PoolingManager.ReturnToPool(voxel);
                    OmiyaGames.Global.PoolingManager.ReturnToPool(this);
                    StopCoroutine(delayDestroyCoroutine);
                }
            }
        }

        IEnumerator DelayDestroy()
        {
            yield return new WaitForSeconds(destroyAfterSeconds);
            OmiyaGames.Global.PoolingManager.ReturnToPool(this);
        }
    }
}
