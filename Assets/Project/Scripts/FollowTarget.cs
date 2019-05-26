using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        // Update is called once per frame
        //void FixedUpdate()
        //{
        //    transform.position = target.position;
        //}
        public void LateUpdate()
        {
            if (target != null)
            {
                transform.position = target.position;
            }
        }
    }
}
