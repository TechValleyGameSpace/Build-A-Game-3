using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using OmiyaGames;
using OmiyaGames.Global;
using OmiyaGames.Audio;

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
        [SerializeField]
        Transform spawnPoint;

        [Header("Animations")]
        [SerializeField]
        float turnSmoothFactor = 10f;
        [SerializeField]
        string propellerSpeedField = "Propeller Speed";
        [SerializeField]
        ParticleSystem jetParticles;

        [Header("Sound Effects")]
        [SerializeField]
        OmiyaGames.Audio.SoundEffect collectibleSound;

        Rigidbody body;
        Vector3 move;
        Animator controller;
        float lastShot = 0f;

        private static readonly HashSet<Collider> playerCollider = new HashSet<Collider>();
        private static PlayerMove instance;

        public static PlayerMove GetPlayer(Collider check)
        {
            if (playerCollider.Contains(check) == true)
            {
                return instance;
            }
            return null;
        }

        public SoundEffect CollectibleSound
        {
            get => collectibleSound;
        }

        // Use this for initialization
        void Start()
        {
            body = GetComponent<Rigidbody>();
            controller = GetComponent<Animator>();
            Collider[] all = GetComponentsInChildren<Collider>();
            playerCollider.Clear();
            foreach (Collider col in all)
            {
                playerCollider.Add(col);
            }
            instance = this;
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

            // Check the speed
            float speed = move.magnitude;
            ParticleSystem.EmissionModule module = jetParticles.emission;
            if (speed > 0.1f)
            {
                // Calculate the rotation
                move.Normalize();
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move, Vector3.up), (Time.deltaTime * turnSmoothFactor));

                // Recalculate move with acceleration
                move.x *= moveForceAcceleration * Time.deltaTime;
                move.y *= moveForceAcceleration * Time.deltaTime;
                move.z *= moveForceAcceleration * Time.deltaTime;
                body.AddForce(move, ForceMode.Acceleration);

                // Animate the propeller
                controller.SetFloat(propellerSpeedField, speed);
                module.enabled = true;
            }
            else
            {
                controller.SetFloat(propellerSpeedField, 0f);
                module.enabled = false;
            }
        }

        private void Update()
        {
            if ((CrossPlatformInputManager.GetButton("Fire1") == true) && ((Time.time - lastShot) > cooldownDurationSeconds))
            {
                Torpedo clone = Singleton.Get<PoolingManager>().GetInstance(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
                //clone.transform.position = transform.position;
                //clone.transform.rotation = transform.rotation;
                lastShot = Time.time;
            }
        }
    }
}
