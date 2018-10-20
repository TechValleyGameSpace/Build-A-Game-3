using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [RequireComponent(typeof(Camera))]
    public class CameraNearPlaneAdjuster : MonoBehaviour
    {
        Camera attachedCamera;

        [SerializeField]
        float nearPlane = 4;
        [SerializeField]
        PlayerMove.Axis perspective;

        // Use this for initialization
        void Start()
        {
            attachedCamera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            float nearPlaneDistance = 0f;
            bool invertDirection = false;
            switch (perspective)
            {
                case PlayerMove.Axis.XY:
                    nearPlaneDistance = transform.position.z;
                    invertDirection = nearPlaneDistance > 0;
                    break;
                case PlayerMove.Axis.XZ:
                    nearPlaneDistance = transform.position.y;
                    invertDirection = nearPlaneDistance > 0;
                    break;
                case PlayerMove.Axis.YZ:
                    nearPlaneDistance = transform.position.x;
                    invertDirection = nearPlaneDistance < 0;
                    break;
            }
            nearPlaneDistance = Mathf.Abs(nearPlaneDistance);
            nearPlaneDistance -= Mathf.Floor(nearPlaneDistance);
            if (invertDirection == true)
            {
                nearPlaneDistance = 1f - nearPlaneDistance;
            }
            attachedCamera.nearClipPlane = nearPlane + nearPlaneDistance;
        }
    }
}
