using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
public partial struct RoterSelectionSystem : ISystem
{
    bool isFirstTouch;
    uint layerRoter;
    
    EntityCommandBuffer ECB;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemActiveEditableComp>();

        isFirstTouch = true;
        layerRoter = 1u << LayerMask.NameToLayer("Roter");
    }

    public void OnUpdate(ref SystemState state)
    {
        ECB = new(Unity.Collections.Allocator.Temp);
        InputManager input = InputManager.Instance;

        if (!input.isFirstTouchPress)
        {
            isFirstTouch = true;
            return;
        }

        if (isFirstTouch) isFirstTouch = false;
        else return;

        float3 screenPos = input.GetFirstScreenTouchPos();
        screenPos.z = 1;
        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(screenPos);

        RoterRayCast(ray.origin, ray.GetPoint(20), ref state);
        
        ECB.Playback(state.EntityManager);
    }

    [BurstCompile]
    void RoterRayCast(float3 rayOrigin, float3 rayEnd, ref SystemState state)
    {
        PhysicsWorldSingleton physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld colWorld = physWorld.CollisionWorld;

        RaycastInput rayCast = new RaycastInput()
        {
            Start = rayOrigin,
            End = rayEnd,
            Filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = layerRoter
            }
        };

        bool isHit = colWorld.CastRay(rayCast, out var hitInfo);
        if (isHit)
        {
            Entity roterEntity = hitInfo.Entity;
            RefRW<LocalTransform> trfm = SystemAPI.GetComponentRW<LocalTransform>(roterEntity);

            float2 hitPos = new() { x = hitInfo.Position.x, y = hitInfo.Position.y };
            float2 center = new() { x = trfm.ValueRO.Position.x, y = trfm.ValueRO.Position.y };

            float dist = math.distancesq(hitPos, center);

            Entity itemEntity = SystemAPI.GetSingletonEntity<ItemActiveEditableComp>();

            if (dist > 0.05f)
            {
                ECB.AddComponent(roterEntity, new RoterRotationComp() { prevVec = hitPos - center });
            }
            else
            {
                ECB.AddComponent<ItemPlaceableComp>(itemEntity);
                ECB.RemoveComponent<ItemPlacedComp>(itemEntity);
            }

            RefRW<RoterDataComp> roterDataComp = SystemAPI.GetComponentRW<RoterDataComp>(roterEntity);
            roterDataComp.ValueRW.isAvailable = false;
        }
    }
}
