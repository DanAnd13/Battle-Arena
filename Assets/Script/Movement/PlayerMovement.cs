using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerMovement : NetworkBehaviour
{
    public PlayerScriptableObject PlayerSettings;
    public Transform cameraPivot;

    private Vector2 lastMouseDelta;
    //[Networked] private Vector3 NetworkedRotation { get; set; }

    private CharacterController controller;
    private float verticalRotation = 0f;

    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private float targetYaw = 0f;
    private float targetPitch = 0f;

    public override void Spawned()
    {
        controller = GetComponent<CharacterController>();

        if (HasInputAuthority)
        {
            // ¬ключенн€ камери або UI дл€ локального гравц€
            Camera.main.transform.SetParent(cameraPivot);
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
        targetPitch -= lastMouseDelta.y * PlayerSettings.RotationSpeed;
        targetPitch = Mathf.Clamp(targetPitch, -45, 45);
        targetYaw += lastMouseDelta.x * PlayerSettings.RotationSpeed;

        // ѕлавне обертанн€ камери
        currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, Time.deltaTime * 10f);
        currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, Time.deltaTime * 10f);

        cameraPivot.localEulerAngles = new Vector3(currentPitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            Vector3 move = transform.forward * input.Movement.y + transform.right * input.Movement.x;
            controller.Move(move * PlayerSettings.MoveSpeed * Runner.DeltaTime);

            if (HasInputAuthority)
                lastMouseDelta = input.MouseDelta;
        }
    }
}
