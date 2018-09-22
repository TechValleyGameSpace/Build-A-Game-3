using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class CameraSelector : MonoBehaviour
    {
        [SerializeField]
        CanvasGroup toggleXY;
        [SerializeField]
        CanvasGroup toggleXZ;
        [SerializeField]
        CanvasGroup toggleYZ;

        private void Start()
        {
            switch(PlayerMove.CurrentAxis)
            {
                case PlayerMove.Axis.XZ:
                    toggleXZ.GetComponent<Toggle>().isOn = true;
                    break;
                case PlayerMove.Axis.YZ:
                    toggleYZ.GetComponent<Toggle>().isOn = true;
                    break;
                default:
                    toggleXY.GetComponent<Toggle>().isOn = true;
                    break;
            }
            UpdateToggles();
        }

        public void OnScreenXYToggle(bool axis)
        {
            if(axis == true)
            {
                PlayerMove.CurrentAxis = PlayerMove.Axis.XY;
                UpdateToggles();
            }
        }

        public void OnScreenXZToggle(bool axis)
        {
            if (axis == true)
            {
                PlayerMove.CurrentAxis = PlayerMove.Axis.XZ;
                UpdateToggles();
            }
        }

        public void OnScreenYZToggle(bool axis)
        {
            if (axis == true)
            {
                PlayerMove.CurrentAxis = PlayerMove.Axis.YZ;
                UpdateToggles();
            }
        }

        private void UpdateToggles()
        {
            toggleXY.alpha = 1;
            toggleXZ.alpha = 1;
            toggleYZ.alpha = 1;
            switch (PlayerMove.CurrentAxis)
            {
                case PlayerMove.Axis.XZ:
                    toggleXZ.alpha = 0;
                    break;
                case PlayerMove.Axis.YZ:
                    toggleYZ.alpha = 0;
                    break;
                default:
                    toggleXY.alpha = 0;
                    break;
            }
        }
    }
}
