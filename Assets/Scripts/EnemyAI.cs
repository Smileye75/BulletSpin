using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    private Transform target;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int currentHealth;

    [SerializeField] private int damage;

    [SerializeField] private Animator animator;

    [Header("Death Settings")]
    [SerializeField] private GameObject disableLight;
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        GetComponent<FlashDamage>()?.TriggerMaterialChange();
        if (currentHealth <= 0)
            Die();
    }




    private void Die()
    {
        animator.SetTrigger("Die");
        // Disable specified GameObject if assigned
        if (disableLight != null)
        {
            disableLight.SetActive(false);
        }

        // Stop chasing the player
        target = null;

        Destroy(gameObject, 1f); // Or play death animation, etc.
    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Keep movement horizontal

        transform.position += direction * speed * Time.deltaTime;

        // Optional: Face the player
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

}
