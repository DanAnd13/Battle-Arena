using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using BattleArena.Parameters;
using BattleArena.InputSynchronize;

namespace BattleArena.Movement
{
    public class PlayerMovement : NetworkBehaviour
    {
        public PlayerScriptableObject PlayerSettings;
        public Transform CameraPosition;

        private Vector2 _lastMouseDelta;
        private CharacterController _controller;
        private float _currentYaw = 0f;
        private float _currentPitch = 0f;
        private float _targetYaw = 0f;
        private float _targetPitch = 0f;

        public override void Spawned()
        {
            _controller = GetComponent<CharacterController>();

            if (HasInputAuthority)
            {
                // ¬ключенн€ камери або UI дл€ локального гравц€
                Camera.main.transform.SetParent(CameraPosition);
                Camera.main.transform.localPosition = Vector3.zero;
                Camera.main.transform.localRotation = Quaternion.identity;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

            }
        }

        private void Update()
        {
            if (!HasInputAuthority) return;

            // «м≥на ц≥льового кута
            _targetPitch -= _lastMouseDelta.y * PlayerSettings.RotationSpeed;
            _targetPitch = Mathf.Clamp(_targetPitch, -45, 45);
            _targetYaw += _lastMouseDelta.x * PlayerSettings.RotationSpeed;

            // ѕлавне обертанн€ камери
            _currentPitch = Mathf.LerpAngle(_currentPitch, _targetPitch, Time.deltaTime * 10f);
            _currentYaw = Mathf.LerpAngle(_currentYaw, _targetYaw, Time.deltaTime * 10f);

            CameraPosition.localEulerAngles = new Vector3(_currentPitch, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, _currentYaw, 0f);
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                Vector3 move = transform.forward * input.Movement.y + transform.right * input.Movement.x;
                _controller.Move(move * PlayerSettings.MoveSpeed * Runner.DeltaTime);

                if (HasInputAuthority)
                    _lastMouseDelta = input.MouseDelta;
            }
        }
    }
}
