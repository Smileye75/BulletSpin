using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(menuName = "BulletData")]
public class BulletData : ScriptableObject
{
    [Header("General")]
    public GameObject bulletPrefab;
    public GameObject imagePrefab;

    [Header("Stats")]
    public float speed = 20f;
    public int damage = 1;

    [Header("Orbit Settings")]
    public int numberOfLoops = 3;
    public float verticalOffset = 1f;
    public float orbitSpeed = 180f;
    public float returnSpeed = 10f;
    public float startRadius = 3f;
    public float endRadius = 1f;

    [Header("Travel Settings")]
    public float travelDistance = 2f;
}
