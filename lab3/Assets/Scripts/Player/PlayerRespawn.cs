using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth), typeof(PlayerController))]
public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("If empty, the object named \"SpawnPoint\" in the scene is used.")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fallBoundaryY = -15f;
    [SerializeField] private float respawnDelay = 0.4f;

    private PlayerHealth health;
    private PlayerController player;
    private PlayerAnimationRelay animationRelay;
    private bool isRespawning;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        player = GetComponent<PlayerController>();
        animationRelay = GetComponent<PlayerAnimationRelay>();
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
    }

    private void OnDisable()
    {
        health.Died -= HandleDeath;
    }

    private void Start()
    {
        if (spawnPoint == null)
        {
            GameObject sp = GameObject.Find("SpawnPoint");
            if (sp != null) spawnPoint = sp.transform;
        }
    }

    private void Update()
    {
        if (!isRespawning && transform.position.y < fallBoundaryY)
        {
            health.Kill();
        }
    }

    private void HandleDeath()
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (animationRelay != null) animationRelay.TriggerDeath();

        player.StopImmediately();
        player.Col.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        transform.localScale = Vector3.one;
        transform.position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        player.StopImmediately();
        player.Col.enabled = true;

        health.ResetHealth();
        player.ForceAirState();

        isRespawning = false;
    }
}
