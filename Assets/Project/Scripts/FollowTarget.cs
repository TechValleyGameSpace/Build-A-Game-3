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
            transform.position = target.position;
        }
    }
}
