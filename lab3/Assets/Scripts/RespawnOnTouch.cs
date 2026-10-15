using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

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
        PlayerController player = target.GetComponent<PlayerController>();

        if (player != null)
        {
            if (spawnPoint != null)
            {
                target.transform.position = spawnPoint.position;
            }
            else
            {
                target.transform.position = Vector3.zero;
            }

            if (player.Rb != null)
            {
                player.Rb.linearVelocity = Vector2.zero;
            }
        }
    }
}