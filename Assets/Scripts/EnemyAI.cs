using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    private Transform target;

    [Header("Health")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    [SerializeField] private int damage = 10;


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
        if (currentHealth <= 0)
            Die();
    }

    public void DealDamage(int amount)
    {

    }

    private void Die()
    {
        Destroy(gameObject); // Or play death animation, etc.
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
