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
        [SerializeField]
        OmiyaGames.Audio.SoundEffect sound;
        [SerializeField]
        ParticleSystem particles;

        Rigidbody body = null;
        Vector3 speedVector;

        public override void Start()
        {
            base.Start();
            body = GetComponent<Rigidbody>();
            speedVector = new Vector3(0f, 0f, speed);
            sound.Play();
            particles.Stop();
            particles.Clear();
            particles.Play();

            StartCoroutine(DelayDestroy(destroyAfterSeconds));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Destructable") == true)
            {
                Voxel voxel = collision.collider.GetComponent<Voxel>();
                if (voxel != null)
                {
                    voxel.ExplodeVoxel();
                }
            }
        }

        private void FixedUpdate()
        {
            body.AddRelativeForce(speedVector, ForceMode.VelocityChange);
        }

        IEnumerator DelayDestroy(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            body.velocity = Vector3.zero;
            OmiyaGames.Global.PoolingManager.ReturnToPool(this);
        }
    }
}
