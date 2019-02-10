using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    public class Collectable : OmiyaGames.PooledObject
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") == true)
            {
                PlayerMove player = PlayerMove.GetPlayer(other);
                if(player != null)
                {
                    player.CollectibleSound.Play();
                }
                PoolingManager.ReturnToPool(this);
            }
        }
    }
}
