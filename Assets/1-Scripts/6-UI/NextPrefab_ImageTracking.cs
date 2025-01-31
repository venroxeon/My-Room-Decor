using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

public class NextPrefab_ImageTracking : MonoBehaviour
{
    [SerializeField] PrefabImagePairManager script;

    public void NextPrefab()
    {
        script.NextPrefab();
    }
}