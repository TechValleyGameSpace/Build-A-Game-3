using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMove : MonoBehaviour
    {
        public enum Axis
        {
            XY,
            XZ,
            YZ
        }

        [SerializeField]
        Axis axis;

        Rigidbody body;

        // Use this for initialization
        void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {

        }
    }
}
