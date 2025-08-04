// ------------------------------------------------------------
// FlashDamage.cs
// Handles temporary material color change for damage feedback.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the material color of child mesh renderers to provide damage feedback.
/// </summary>
public class FlashDamage : MonoBehaviour
{
    private List<Material[]> originalMaterials = new List<Material[]>(); // Original materials for each renderer
    private List<Color[]> originalColors = new List<Color[]>();          // Original colors for each renderer
    private Coroutine changeCoroutine;                                   // Coroutine for flashing
    private Coroutine revertCoroutine;                                   // Coroutine for reverting

    /// <summary>
    /// Stores original materials and colors on start.
    /// </summary>
    protected virtual void Start()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            originalMaterials.Add(renderer.materials);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            originalColors.Add(colors.ToArray());
        }
    }

    /// <summary>
    /// Triggers the material color change for damage feedback.
    /// </summary>
    public void TriggerMaterialChange()
    {
        if (changeCoroutine != null)
        {
            StopCoroutine(changeCoroutine);
        }

        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
        }
        changeCoroutine = StartCoroutine(ChangeMaterialsTemporarily());
    }

    /// <summary>
    /// Changes materials to flash color, then reverts after a delay.
    /// </summary>
    private IEnumerator ChangeMaterialsTemporarily()
    {
        ChangeMaterialsToColor(new Color(2, 2, 2)); // Flash white
        yield return new WaitForSeconds(0.2f);
        revertCoroutine = StartCoroutine(RevertMaterialsSmoothly(1.0f));
    }

    /// <summary>
    /// Changes all materials to the specified color.
    /// </summary>
    private void ChangeMaterialsToColor(Color color)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                // Create a new temporary material instance
                newMaterials[i] = new Material(renderer.materials[i]);
                if (newMaterials[i].HasProperty("_Color"))
                {
                    newMaterials[i].color = color;
                }
            }
            renderer.materials = newMaterials;
        }
    }

    /// <summary>
    /// Smoothly reverts materials back to their original colors.
    /// </summary>
    private IEnumerator RevertMaterialsSmoothly(float duration)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        List<Color[]> currentColors = new List<Color[]>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            currentColors.Add(colors.ToArray());
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                for (int j = 0; j < meshRenderers[i].materials.Length; j++)
                {
                    if (meshRenderers[i].materials[j].HasProperty("_Color"))
                    {
                        meshRenderers[i].materials[j].color = Color.Lerp(Color.white, originalColors[i][j], t);
                    }
                }
            }

            yield return null;
        }

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].materials = originalMaterials[i];
        }
    }
}
