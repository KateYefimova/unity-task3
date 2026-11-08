using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryRespawn(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryRespawn(collision.gameObject);
    }

    private void TryRespawn(GameObject target)
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.RespawnAtCheckpoint();
        }
    }
}