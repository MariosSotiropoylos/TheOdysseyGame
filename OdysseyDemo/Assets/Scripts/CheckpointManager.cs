using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("PLAYER")]
    public Transform player;

    [Header("ORIGINAL SPAWNING LOCATION")]
    public Transform originalSpawnPoint;

    [Header("RUNTIME INFO")]
    public Transform currentCheckpoint;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (originalSpawnPoint != null)
        {
            currentCheckpoint = originalSpawnPoint;
        }
        else if (player != null)
        {
            currentCheckpoint = null;
        }
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        if (newCheckpoint == null)
        {
            return;
        }

        currentCheckpoint = newCheckpoint;
    }

    public Vector3 GetRespawnPosition()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.position;
        }

        if (originalSpawnPoint != null)
        {
            return originalSpawnPoint.position;
        }

        if (player != null)
        {
            return player.position;
        }

        return Vector3.zero;
    }

    public Quaternion GetRespawnRotation()
    {
        if (currentCheckpoint != null)
        {
            return currentCheckpoint.rotation;
        }

        if (originalSpawnPoint != null)
        {
            return originalSpawnPoint.rotation;
        }

        if (player != null)
        {
            return player.rotation;
        }

        return Quaternion.identity;
    }
}