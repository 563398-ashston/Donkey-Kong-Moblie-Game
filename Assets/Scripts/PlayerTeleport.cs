
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleport : MonoBehaviour
{
    private GameObject currentTeleporter;

    public InputActionReference interact;

    private void OnEnable()
    {
        interact.action.Enable();
    }

    private void OnDisable()
    {
        interact.action.Disable();
    }

    void Update()
    {
        if (interact.action.WasPressedThisFrame())
        {
            if (currentTeleporter != null)
            {
                transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            if (collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }
}
