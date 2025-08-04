// ------------------------------------------------------------
// MainMenu.cs
// Handles main menu and scene transitions for the game.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles main menu button actions and scene transitions.
/// </summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Starts the game by loading the main gameplay scene.
    /// </summary>
    public void Play()
    {
        Time.timeScale = 1f;
        BulletShooter.isUpgrading = false; // Allow shooting when game starts
        SceneManager.LoadScene("BulletLoop");
        
    }

    /// <summary>
    /// Restarts the current scene (try again after defeat).
    /// </summary>
    public void TryAgain()
    {
        Time.timeScale = 1f; // Reset time scale to normal
        BulletShooter.isUpgrading = false; // Allow shooting when retrying
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Returns to the main menu scene.
    /// </summary>
    public void Menu()
    {
        Time.timeScale = 1f; // Reset time scale to normal
        BulletShooter.isUpgrading = false; // Allow shooting in menu
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Exits the game application.
    /// </summary>
    public void Quit()
    {
        Time.timeScale = 1f; // Reset time scale to normal
        Application.Quit();
    }
}
