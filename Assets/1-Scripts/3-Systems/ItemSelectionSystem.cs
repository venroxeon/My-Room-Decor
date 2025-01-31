using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
public partial struct ItemSelectionSystem : ISystem
{
    bool isFirstTouch;
    uint layerPlaceables;
    EntityCommandBuffer ECB;
    
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemPlacedComp>();

        layerPlaceables = 1u << LayerMask.NameToLayer("Placeables");
    }

    void OnUpdate(ref SystemState state)
    {
        InputManager input = InputManager.Instance;
        
        if (!input.isFirstTouchPress)
        {
            isFirstTouch = true;
            return;
        }

        if (isFirstTouch) isFirstTouch = false;
        else return;

        ECB = new(Unity.Collections.Allocator.Temp);
        
        float3 screenPos = input.GetFirstScreenTouchPos();
        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(screenPos);

        ItemSelectionRayCast(ray.origin, ray.GetPoint(20), ref state);
        
        ECB.Playback(state.EntityManager);
    }

    void ItemSelectionRayCast(float3 rayOrigin, float3 rayEnd, ref SystemState state)
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
                CollidesWith = layerPlaceables
            }
        };

        bool isHit = colWorld.CastRay(rayCast, out var hitInfo);

        if (isHit)
        {
            Entity roterEntity = SystemAPI.GetSingletonEntity<RoterDataComp>();
            RefRW<RoterDataComp> roterDataComp = SystemAPI.GetComponentRW<RoterDataComp>(roterEntity);
            RefRW<LocalTransform> roterTrfm = SystemAPI.GetComponentRW<LocalTransform>(roterEntity);

            roterDataComp.ValueRW.isAvailable = false;
            
            LocalTransform itemTrfm = SystemAPI.GetComponent<LocalTransform>(hitInfo.Entity);

            float3 screenPos = Camera.main.WorldToScreenPoint(itemTrfm.Position);
            screenPos.y -= Utility.touchDeltaY;
            screenPos.z = 1;
            
            float3 newPos = Camera.main.ScreenToWorldPoint(screenPos);
            
            roterTrfm.ValueRW.Position = newPos;

            if (SystemAPI.HasSingleton<ItemActiveEditableComp>()) ECB.RemoveComponent<ItemActiveEditableComp>(SystemAPI.GetSingletonEntity<ItemActiveEditableComp>());
            ECB.AddComponent<ItemActiveEditableComp>(hitInfo.Entity);
        }
    }
}
