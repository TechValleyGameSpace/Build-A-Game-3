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
            Debug.Log("Yay 1!");
            if (other.CompareTag("Player") == true)
            {
                Debug.Log("Yay 2!");
                PoolingManager.ReturnToPool(this);
            }
        }
    }
}
