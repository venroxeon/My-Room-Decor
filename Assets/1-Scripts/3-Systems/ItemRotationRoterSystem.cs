using Unity.Transforms;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct ItemRotationRoterSystem : ISystem
{
    EntityCommandBuffer ECB;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RoterRotationComp>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ECB = new(Unity.Collections.Allocator.Temp);
        InputManager input = InputManager.Instance;

        if (input.isFirstTouchPress)
        {
            float3 touchPos = input.GetFirstTouchPos(1);
            ItemRotation(new float2() { x = touchPos.x, y = touchPos.y }, ref state);
        }
        else
        {
            RemoveRotation(ref state);
        }
        
        ECB.Playback(state.EntityManager);
    }

    [BurstCompile]
    void ItemRotation(float2 touchPos, ref SystemState state)
    {
        foreach (var (roterDataComp, rotRoterComp, roterTrfm) in SystemAPI.Query<RoterDataComp, RefRW<RoterRotationComp>, RefRW<LocalTransform>>())
        {
            float2 center = new() { x = roterTrfm.ValueRO.Position.x, y = roterTrfm.ValueRO.Position.y };
            float2 touchVec = touchPos - center;
            float2 prevVec = rotRoterComp.ValueRO.prevVec;

            if (math.Equals(touchVec, prevVec)) return;

            float angle = GetAngle(touchVec, prevVec);

            RefRW<LocalTransform> itemTrfm = SystemAPI.GetComponentRW<LocalTransform>(SystemAPI.GetSingletonEntity<ItemActiveEditableComp>());

            itemTrfm.ValueRW = itemTrfm.ValueRO.RotateY(angle);
            roterTrfm.ValueRW = roterTrfm.ValueRO.RotateZ(-angle);

            rotRoterComp.ValueRW.prevVec = touchVec;
        }
    }

    [BurstCompile]
    void RemoveRotation(ref SystemState state)
    {
        Entity roterEntity = SystemAPI.GetSingletonEntity<RoterRotationComp>();

        ECB.RemoveComponent<RoterRotationComp>(roterEntity);
    }

    [BurstCompile]
    float GetAngle(float2 vecA, float2 vecB)
    {
        float dot = math.dot(vecA, vecB);
        float cross = vecA.x * vecB.y - vecA.y * vecB.x;

        float angle = math.atan2(cross, dot);

        return angle;
    }
}