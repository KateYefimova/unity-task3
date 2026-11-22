using UnityEngine;

/// <summary>Hazard (spikes, pit): touching it is always lethal for the player.</summary>
public class RespawnOnTouch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryKill(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryKill(collision.gameObject);
    }

    private void TryKill(GameObject target)
    {
        PlayerHitHandler hitHandler = target.GetComponent<PlayerHitHandler>();

        if (hitHandler != null)
        {
            hitHandler.ReceiveLethalHit();
        }
    }
}
