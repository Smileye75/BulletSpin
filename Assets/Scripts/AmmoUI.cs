using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject bulletSlotPrefab;
    [SerializeField] private Transform extraParentContainer; // Layout Group for extra slots
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform nextPosition;
    [SerializeField] private AmmoSlot ammoSlot;

    private GameObject prevSlotUI;
    private GameObject currentSlotUI;
    private GameObject nextSlotUI;

    private List<GameObject> extraSlotsUI = new();
    private int currentIndex = 0;

    void Start()
    {
        SetupSlots();
        UpdateVisuals();
    }

    void Update()
    {
        UpdateVisuals();
    }

    public void RotateUIToNext()
    {
        var bullets = ammoSlot.GetAllBullets();
        if (bullets.Count == 0) return;
        currentIndex = (currentIndex + 1) % bullets.Count;
        UpdateVisuals();
    }

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

    public int GetCurrentIndex() => currentIndex;
}
