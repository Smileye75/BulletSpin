// ------------------------------------------------------------
// AmmoUI.cs
// Handles the UI display and cycling for bullet slots.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages bullet slot UI, scaling, and cycling visuals.
/// </summary>
public class AmmoUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject bulletSlotPrefab; // Prefab for bullet slot UI
    [SerializeField] private Transform extraParentContainer; // Layout Group for extra slots
    [SerializeField] private Transform centerPosition; // Center slot position
    [SerializeField] private Transform nextPosition; // Next slot position
    [SerializeField] private AmmoSlot ammoSlot; // Reference to AmmoSlot logic

    private GameObject prevSlotUI;
    private GameObject currentSlotUI;
    private GameObject nextSlotUI;

    private List<GameObject> extraSlotsUI = new(); // Extra slots for hidden bullets
    private int currentIndex = 0; // Current bullet index

    /// <summary>
    /// Initializes bullet slot UI on start.
    /// </summary>
    void Start()
    {
        SetupSlots();
        UpdateVisuals();
    }

    /// <summary>
    /// Updates bullet slot visuals every frame.
    /// </summary>
    void Update()
    {
        UpdateVisuals();
    }

    /// <summary>
    /// Rotates UI to the next bullet slot.
    /// </summary>
    public void RotateUIToNext()
    {
        var bullets = ammoSlot.GetAllBullets();
        if (bullets.Count == 0) return;
        currentIndex = (currentIndex + 1) % bullets.Count;
        UpdateVisuals();
    }

    /// <summary>
    /// Instantiates and sets up bullet slot UI objects.
    /// </summary>
    void SetupSlots()
    {
        // Remove previous slot, only use extra slots
        currentSlotUI = Instantiate(bulletSlotPrefab, centerPosition.position, Quaternion.identity, transform);
        nextSlotUI = Instantiate(bulletSlotPrefab, nextPosition.position, Quaternion.identity, transform);

        // Setup extra slots (will be reused)
        for (int i = 0; i < 5; i++) // You can adjust the number of extra slots
        {
            GameObject extraSlot = Instantiate(bulletSlotPrefab, extraParentContainer);
            extraSlotsUI.Add(extraSlot);
        }
    }

    /// <summary>
    /// Updates the visuals for all bullet slots, scaling and coloring as needed.
    /// </summary>
    void UpdateVisuals()
    {
        var bullets = ammoSlot.GetAllBullets();
        var availability = ammoSlot.GetBulletAvailability();
        int count = bullets.Count;
        if (count < 1) return;

        int nextIndex = (currentIndex + 1) % count;

        float extraScale = 0.7f;
        float nextScale = 0.9f;
        SetBulletVisual(currentSlotUI.GetComponent<Image>(), bullets[currentIndex], availability[currentIndex], 1.2f);
        SetBulletVisual(nextSlotUI.GetComponent<Image>(), bullets[nextIndex], availability[nextIndex], nextScale);

        // Fill extra slots (all not current, next) in reverse order
        int extraSlotIdx = 0;
        for (int i = count - 1; i >= 0; i--)
        {
            if (i == currentIndex || i == nextIndex) continue;
            if (extraSlotIdx >= extraSlotsUI.Count) break;
            SetBulletVisual(extraSlotsUI[extraSlotIdx].GetComponent<Image>(), bullets[i], availability[i], extraScale);
            extraSlotsUI[extraSlotIdx].SetActive(true);
            extraSlotIdx++;
        }
        // Hide and clear unused extra slots
        for (; extraSlotIdx < extraSlotsUI.Count; extraSlotIdx++)
        {
            extraSlotsUI[extraSlotIdx].SetActive(false);
            var img = extraSlotsUI[extraSlotIdx].GetComponent<Image>();
            if (img != null)
            {
                img.sprite = null;
                img.color = Color.clear;
            }
        }
    }

    /// <summary>
    /// Sets the visual appearance of a bullet slot (sprite, color, scale).
    /// </summary>
    void SetBulletVisual(Image image, BulletData data, bool available, float scale)
    {
        if (image == null || data.imagePrefab == null) return;
        var prefabImage = data.imagePrefab.GetComponent<Image>();
        if (prefabImage != null)
        {
            image.sprite = prefabImage.sprite;
            image.color = available ? Color.white : Color.gray;
            image.transform.localScale = Vector3.one * scale;
        }
    }

    /// <summary>
    /// Returns the current bullet index.
    /// </summary>
    public int GetCurrentIndex() => currentIndex;
}
