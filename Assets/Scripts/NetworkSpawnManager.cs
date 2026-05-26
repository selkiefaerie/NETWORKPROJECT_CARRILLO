using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnManager : NetworkBehaviour
{
    private static int nextSpawnIndex;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            MoveToSpawnPoint();
        }

        if (IsOwner)
        {
            SetupLocalCamera();
        }
    }

    private void MoveToSpawnPoint()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No SpawnPoint objects found in the scene.");
            return;
        }

        Transform spawnPoint = spawnPoints[nextSpawnIndex].transform;
        nextSpawnIndex++;
        if (nextSpawnIndex >= spawnPoints.Length)
        {
            nextSpawnIndex = 0;
        }

        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    private void SetupLocalCamera()
    {
        if (Camera.main == null)
        {
            return;
        }

        MainCameraFollow cameraFollow = Camera.main.GetComponent<MainCameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetFollowTarget(transform);
        }
    }
}
