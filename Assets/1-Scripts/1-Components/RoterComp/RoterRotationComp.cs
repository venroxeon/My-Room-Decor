using Unity.Entities;
using Unity.Mathematics;

public struct RoterRotationComp : IComponentData
{
    public float2 prevVec;
}