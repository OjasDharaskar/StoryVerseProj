
using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform spawnPoint;
    public string alienTag = "Enemy";
    public float respawnDelay = 1f;

    private CharacterController characterController;
    private bool isRespawning;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (spawnPoint != null)
            RespawnNow();
        else
            Debug.LogWarning("Assign PlayerSpawnPoint in the Inspector.");
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag(alienTag))
            StartRespawn();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(alienTag))
            StartRespawn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(alienTag))
            StartRespawn();
    }

    void StartRespawn()
    {
        if (!isRespawning)
            StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        RespawnNow();

        isRespawning = false;
    }

    void RespawnNow()
    {
        if (spawnPoint == null)
            return;

        if (characterController != null)
            characterController.enabled = false;

        transform.SetPositionAndRotation(
            spawnPoint.position,
            spawnPoint.rotation
        );

        if (characterController != null)
            characterController.enabled = true;
    }
}
