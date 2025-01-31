using Unity.Entities;
using Unity.Mathematics;

public struct RoomPosAndRotComp : IComponentData
{
    public float3 pos;
    public quaternion rot;
}