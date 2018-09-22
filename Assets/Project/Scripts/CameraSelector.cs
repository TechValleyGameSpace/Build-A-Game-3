using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class CameraSelector : MonoBehaviour
    {
        [SerializeField]
        Button toggleXY;
        [SerializeField]
        Button toggleXZ;
        [SerializeField]
        Button toggleYZ;

        private void Start()
        {
            UpdateToggles();
        }

        public void OnScreenXYToggle(bool axis)
        {
            if(axis == true)
            {
                OnScreenClicked(PlayerMove.Axis.XY);
            }
        }

        public void OnScreenXZToggle(bool axis)
        {
            if (axis == true)
            {
                OnScreenClicked(PlayerMove.Axis.XZ);
            }
        }

        public void OnScreenYZToggle(bool axis)
        {
            if (axis == true)
            {
                OnScreenClicked(PlayerMove.Axis.YZ);
            }
        }

        public void OnScreenClicked(int axis)
        {
            OnScreenClicked((PlayerMove.Axis)axis);
        }

        public void OnScreenClicked(PlayerMove.Axis axis)
        {
            PlayerMove.CurrentAxis = axis;
            UpdateToggles();
        }

        private void UpdateToggles()
        {
            toggleXY.interactable = true;
            toggleXZ.interactable = true;
            toggleYZ.interactable = true;
            switch (PlayerMove.CurrentAxis)
            {
                case PlayerMove.Axis.XZ:
                    toggleXZ.interactable = false;
                    break;
                case PlayerMove.Axis.YZ:
                    toggleYZ.interactable = false;
                    break;
                default:
                    toggleXY.interactable = false;
                    break;
            }
        }
    }
}
