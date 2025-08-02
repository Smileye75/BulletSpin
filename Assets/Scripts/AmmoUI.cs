using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject bulletSlotPrefab;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform nextPosition;
    [SerializeField] private AmmoSlot ammoSlot;

    private GameObject currentSlotUI;
    private GameObject nextSlotUI;

    private Image currentImage;
    private Image nextImage;

    private int currentIndex = 0;

    public static object Instance { get; internal set; }

    void Start()
    {
        SetupInitialSlots();
    }

    void Update()
    {
        UpdateSlotVisuals();
    }

    void SetupInitialSlots()
    {
        var bullets = ammoSlot.GetAllBullets();
        if (bullets.Count < 2) return;

        currentSlotUI = Instantiate(bulletSlotPrefab, centerPosition.position, Quaternion.identity, transform);
        nextSlotUI = Instantiate(bulletSlotPrefab, nextPosition.position, Quaternion.identity, transform);

        currentImage = currentSlotUI.GetComponent<Image>();
        nextImage = nextSlotUI.GetComponent<Image>();

        UpdateSlotVisuals();
    }

    public void RotateUIToNext()
    {
        var bullets = ammoSlot.GetAllBullets();
        if (bullets.Count == 0) return;

        currentIndex = (currentIndex + 1) % bullets.Count;

        // Swap GameObject references
        GameObject tempObj = currentSlotUI;
        currentSlotUI = nextSlotUI;
        nextSlotUI = tempObj;

        // Swap image references
        Image tempImg = currentImage;
        currentImage = nextImage;
        nextImage = tempImg;

        // Update positions
        currentSlotUI.transform.position = centerPosition.position;
        nextSlotUI.transform.position = nextPosition.position;

        UpdateSlotVisuals();
    }

void UpdateSlotVisuals()
{
    var bullets = ammoSlot.GetAllBullets();
    if (bullets.Count == 0) return;

    int nextIndex = (currentIndex + 1) % bullets.Count;

    // Destroy old children (icon objects)
    foreach (Transform child in currentSlotUI.transform)
        Destroy(child.gameObject);
    foreach (Transform child in nextSlotUI.transform)
        Destroy(child.gameObject);

    // Instantiate the imagePrefab for current and next bullet, and set color based on availability
    var availability = ammoSlot.GetBulletAvailability();
    if (bullets[currentIndex].imagePrefab != null)
    {
        var iconObj = Instantiate(bullets[currentIndex].imagePrefab, currentSlotUI.transform);
        iconObj.transform.localPosition = Vector3.zero;
        iconObj.transform.localScale = Vector3.one;
        var img = iconObj.GetComponent<Image>();
        if (img != null)
            img.color = availability[currentIndex] ? Color.white : Color.gray;
    }
    if (bullets[nextIndex].imagePrefab != null)
    {
        var iconObj = Instantiate(bullets[nextIndex].imagePrefab, nextSlotUI.transform);
        iconObj.transform.localPosition = Vector3.zero;
        iconObj.transform.localScale = Vector3.one;
        var img = iconObj.GetComponent<Image>();
        if (img != null)
            img.color = availability[nextIndex] ? Color.white : Color.gray;
    }

    currentSlotUI.transform.localScale = Vector3.one * 1.2f;
    nextSlotUI.transform.localScale = Vector3.one * 0.8f;
}

    public int GetCurrentIndex() => currentIndex;
}
