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
        [SerializeField]
        float speed = 10f;

        Rigidbody body = null;
        Vector3 speedVector;

        public override void Start()
        {
            base.Start();
            body = GetComponent<Rigidbody>();
            speedVector = new Vector3(0f, 0f, speed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Destructable") == true)
            {
                Voxel voxel = collision.collider.GetComponent<Voxel>();
                if (voxel != null)
                {
                    OmiyaGames.Global.PoolingManager.ReturnToPool(voxel);
                }
            }
        }

        private void FixedUpdate()
        {
            body.AddRelativeForce(speedVector, ForceMode.VelocityChange);
        }

        IEnumerator DelayDestroy()
        {
            yield return new WaitForSeconds(destroyAfterSeconds);
            OmiyaGames.Global.PoolingManager.ReturnToPool(this);
        }
    }
}
