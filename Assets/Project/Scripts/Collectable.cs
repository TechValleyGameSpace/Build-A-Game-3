using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    public class Collectable : PooledObject
    {
        [SerializeField]
        private Color flashScreenColor = Color.white;

        public StoryHistory Logs
        {
            private get;
            set;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") == true)
            {
                PlayerMove player = PlayerMove.GetPlayer(other);
                if(player != null)
                {
                    player.CollectibleSound.Play();
                    Singleton.Get<CameraManager>().Effects.FlashOnce(flashScreenColor);
                    Singleton.Get<TimeManager>().HitPause();
                }
                if(Logs != null)
                {
                    Logs.GetNextLog();
                }
                PoolingManager.ReturnToPool(this);
            }
        }
    }
}
