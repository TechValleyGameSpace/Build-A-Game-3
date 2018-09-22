using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Project
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMove : MonoBehaviour
    {
        public enum Axis
        {
            XY = 0,
            XZ,
            YZ
        }

        public static Axis CurrentAxis
        {
            get;
            set;
        }

        [SerializeField]
        float moveForceAcceleration = 10f;

        [Header("Animations")]
        [SerializeField]
        float turnSmoothFactor = 10f;

        Rigidbody body;
        Vector3 move;
        Vector3 forward;

        // Use this for initialization
        void Start()
        {
            body = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            switch (CurrentAxis)
            {
                case Axis.XZ:
                    move.x = horizontal;
                    move.y = 0;
                    move.z = vertical;
                    break;
                case Axis.YZ:
                    move.x = 0;
                    move.y = vertical;
                    move.z = horizontal;
                    break;
                default:
                    move.x = horizontal;
                    move.y = vertical;
                    move.z = 0;
                    break;
            }

            if(move.sqrMagnitude > 0.01f)
            {
                forward = move.normalized;
                transform.forward = Vector3.Lerp(transform.forward, forward, (Time.deltaTime * turnSmoothFactor));
            }

            // Recalculate move with acceleration
            move.x *= moveForceAcceleration * Time.deltaTime;
            move.y *= moveForceAcceleration * Time.deltaTime;
            move.z *= moveForceAcceleration * Time.deltaTime;
            body.AddForce(move, ForceMode.Acceleration);
        }
    }
}
