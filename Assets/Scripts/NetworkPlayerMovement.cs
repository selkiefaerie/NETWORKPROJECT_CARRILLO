using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float groundedGravity = -2f;
    [SerializeField] private float jumpHeight = 2f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        // Remote players are moved by NetworkTransform — CharacterController would fight that
        if (!IsOwner && controller != null)
        {
            controller.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        Vector2 inputDirection = NetworkPlayerInput.ReadMove();
        bool jumpPressed = NetworkPlayerInput.WasJumpPressed();

        if (IsServer)
        {
            ApplyMovement(inputDirection, jumpPressed);
        }
        else
        {
            // Move locally for smooth control, server still runs the same logic from the RPC
            ApplyMovement(inputDirection, jumpPressed);
            SendMovementToServerRpc(inputDirection, jumpPressed);
        }
    }

    [Rpc(SendTo.Server)]
    private void SendMovementToServerRpc(Vector2 inputDirection, bool jumpPressed)
    {
        ApplyMovement(inputDirection, jumpPressed);
    }

    private void ApplyMovement(Vector2 inputDirection, bool jumpPressed)
    {
        if (controller == null || !controller.enabled)
        {
            return;
        }

        if (Camera.main != null)
        {
            float cameraY = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, cameraY, 0f);
        }

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = groundedGravity;
            }

            if (jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 moveDirection = transform.forward * inputDirection.y + transform.right * inputDirection.x;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 velocity = moveDirection * moveSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
