using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using OmiyaGames;
using OmiyaGames.Global;

namespace Project
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
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
        } = Axis.XZ;

        [SerializeField]
        float moveForceAcceleration = 10f;

        [Header("Projectiles")]
        [SerializeField]
        Torpedo projectilePrefab;
        [SerializeField]
        float cooldownDurationSeconds = 0.5f;

        [Header("Animations")]
        [SerializeField]
        float turnSmoothFactor = 10f;
        [SerializeField]
        string propellerSpeedField = "Propeller Speed";

        Rigidbody body;
        Vector3 move;
        Animator controller;
        float lastShot = 0f;

        // Use this for initialization
        void Start()
        {
            body = GetComponent<Rigidbody>();
            controller = GetComponent<Animator>();
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

            if (move.sqrMagnitude > 0.01f)
            {
                move.Normalize();
                transform.forward = Vector3.Lerp(transform.forward, move, (Time.deltaTime * turnSmoothFactor));
                controller.SetFloat(propellerSpeedField, move.magnitude);
            }
            else
            {
                controller.SetFloat(propellerSpeedField, 0f);
            }

            // Recalculate move with acceleration
            move.x *= moveForceAcceleration * Time.deltaTime;
            move.y *= moveForceAcceleration * Time.deltaTime;
            move.z *= moveForceAcceleration * Time.deltaTime;
            body.AddForce(move, ForceMode.Acceleration);
        }

        private void Update()
        {
            if ((CrossPlatformInputManager.GetButton("Fire1") == true) && ((Time.time - lastShot) > cooldownDurationSeconds))
            {
                Torpedo clone = Singleton.Get<PoolingManager>().GetInstance(projectilePrefab, transform.position, transform.rotation);
                clone.transform.position = transform.position;
                clone.transform.rotation = transform.rotation;
                clone.transform.localScale = Vector3.one;
                lastShot = Time.time;
            }
        }
    }
}
