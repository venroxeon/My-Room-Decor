using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ItemAnimSelectedComp : IComponentData
{
    public bool isScaled, isFloored, isOverlapping, _isOverlapping, isReverted;
    public float upScale;
    public float3 roterPos;
    public uint targetLayer;
}