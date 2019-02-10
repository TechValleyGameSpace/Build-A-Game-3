using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    public class PooledEmitOnceParticles : PooledObject
    {
        [SerializeField]
        ParticleSystem particles;

        public override void Start()
        {
            base.Start();
            if(particles == null)
            {
                particles = GetComponent<ParticleSystem>();
            }
            if (particles == null)
            {
                particles.Play();
                StartCoroutine(DisableAfter(particles.main.duration));
            }
        }

        IEnumerator DisableAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            PoolingManager.ReturnToPool(this);
        }
    }
}
