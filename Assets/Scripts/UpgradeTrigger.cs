// ------------------------------------------------------------
// UpgradeTrigger.cs
// Handles upgrade event trigger, UI, and gameplay pause/resume.
// ------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Triggers upgrade UI and logic when the player enters the upgrade area.
/// Pauses gameplay, heals player, and resumes after upgrade.
/// </summary>
public class UpgradeTrigger : MonoBehaviour
{
    [SerializeField] private GameObject upgradeUI; // Assign your upgrade UI Canvas or Panel
    [SerializeField] private GameObject player;    // Assign the player GameObject

    private bool triggered = false;                // Prevents multiple triggers

    /// <summary>
    /// Handles player entry: pauses game, heals player, and shows upgrade UI.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            if (player != null)
            {
                player.GetComponent<PlayerStateMachine>().enabled = false;
            }
            triggered = true;
            // Prevent shooting while upgrading
            BulletShooter.isUpgrading = true;
            // Heal the player
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.Heal(health.maxHealth); // Heal to full, or set a value as needed
            // Show upgrade UI
            if (upgradeUI != null)
                upgradeUI.SetActive(true);

            // Stop time
            Time.timeScale = 0f;
        }
    }

    /// <summary>
    /// Call this when upgrade is finished to resume gameplay.
    /// Hides upgrade UI, resumes game, and resets trigger state.
    /// </summary>
    public void ResumeGame()
    {
        if (upgradeUI != null)
            upgradeUI.SetActive(false);
        FindObjectOfType<GameManager>().OnUpgradeResume();
        if (player != null)
            player.GetComponent<PlayerStateMachine>().enabled = true;
        // Allow shooting again after upgrade
        BulletShooter.isUpgrading = false;
        Time.timeScale = 1f;
        triggered = false;
    }
}
