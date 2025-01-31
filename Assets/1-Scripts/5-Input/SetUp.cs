using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp : MonoBehaviour
{
    [SerializeField] int frameRate;
    private void Awake()
    {
        Application.targetFrameRate = frameRate;
    }
}
