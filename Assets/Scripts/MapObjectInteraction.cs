using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class MapObjectInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The tag of the object that can trigger this interaction (usually the Player).")]
    public string playerTag = "Player";

    [Tooltip("If true, the player must press the Interact Key while in range. If false, simply touching the object triggers it.")]
    public bool requireKeyPress = true;
    public KeyCode interactKey = KeyCode.E;

    [Header("Events")]
    [Tooltip("Fires when the interaction successfully occurs.")]
    public UnityEvent onInteract;

    [Tooltip("Fires when the player enters the interaction zone (useful for showing UI prompts).")]
    public UnityEvent onPlayerEnter;

    [Tooltip("Fires when the player leaves the interaction zone (useful for hiding UI prompts).")]
    public UnityEvent onPlayerExit;

    private bool isPlayerInRange = false;

    private void Update()
    {
        // Check if player is in range, requires a key press, and the E key was pressed this frame
        if (isPlayerInRange && requireKeyPress && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void Interact()
    {
        Debug.Log($"Interacted with {gameObject.name}");
        onInteract?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            onPlayerEnter?.Invoke();

            // Auto-interact if no key press is required (e.g., a pressure plate or trap)
            if (!requireKeyPress)
            {
                Interact();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            onPlayerExit?.Invoke();
        }
    }
}
