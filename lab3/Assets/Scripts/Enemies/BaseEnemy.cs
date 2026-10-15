using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 3f;
    protected Rigidbody2D rb;
    protected Transform playerTransform;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    protected abstract void UpdateBehaviour();

    protected virtual void Update()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            return;
        }
        UpdateBehaviour();
    }
}