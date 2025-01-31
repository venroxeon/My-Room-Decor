using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
public partial struct ItemPlacementSystem : ISystem
{
    bool isTouchActive;
    EntityCommandBuffer ECB;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemPlaceableComp>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ECB = new(Unity.Collections.Allocator.Temp);
        InputManager input = InputManager.Instance;

        if (input.isFirstTouchPress)
        {
            float3 screenPos = input.GetFirstScreenTouchPos();
            screenPos.y += Utility.touchDeltaY;
            screenPos.z = 1;
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(screenPos);

            Placement(Camera.main.ScreenToWorldPoint(screenPos), ray.origin, ray.GetPoint(20), Camera.main.transform.rotation, ref state);
            
            isTouchActive = true;
        }
        else if(isTouchActive)
        {
            Placed(ref state);
            
            isTouchActive = false;
        }
        
        ECB.Playback(state.EntityManager);
    }

    [BurstCompile]
    void Placement(float3 touchPosMod, float3 rayOrigin, float3 rayEnd, quaternion cameraRot, ref SystemState state)
    {
        foreach (var (itemAnimSelectedComp, itemTrfm) in SystemAPI.Query<RefRW<ItemAnimSelectedComp>, RefRW<LocalTransform>>().WithAll<ItemPlaceableComp>())
        {
            RaycastInput rayCast = new()
            {
                Start = rayOrigin,
                End = rayEnd,
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = itemAnimSelectedComp.ValueRO.targetLayer
                }
            };

            PhysicsWorldSingleton physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld colWorld = physWorld.CollisionWorld;

            bool isHit = colWorld.CastRay(rayCast, out var hitInfo);

            // quaternion roomRot = SystemAPI.GetComponent<LocalTransform>(SystemAPI.GetSingletonEntity<RoomComp>()).Rotation;
            
            if (isHit)
            {
                float3 pos = hitInfo.Position;
                itemTrfm.ValueRW.Position = pos;
                // itemTrfm.ValueRW.Rotation = quaternion.LookRotation(math.forward(roomRot), math.mul(roomRot, new float3(0, 1, 0)));
                itemAnimSelectedComp.ValueRW.isFloored = true;
            }
            else
            {
                itemTrfm.ValueRW.Position = touchPosMod;
                // itemTrfm.ValueRW.Rotation = quaternion.LookRotation(math.forward(roomRot), math.mul(roomRot, new float3(0, 1, 0)));

                itemAnimSelectedComp.ValueRW.isFloored = false;
            }
        }

        // RefRW<RoterDataComp> roterDataComp = SystemAPI.GetSingletonRW<RoterDataComp>();
        // roterDataComp.ValueRW.isAvailable = false;
    }

    [BurstCompile]
    void Placed(ref SystemState state)
    {
        Entity itemEntity = SystemAPI.GetSingletonEntity<ItemPlaceableComp>();
        RefRW<ItemAnimSelectedComp> ItemAnimSelectedComp = SystemAPI.GetComponentRW<ItemAnimSelectedComp>(itemEntity);

        if (!ItemAnimSelectedComp.ValueRO.isFloored)
        {
            ECB.DestroyEntity(itemEntity);

            RefRW<LocalTransform> roterTrfm = SystemAPI.GetComponentRW<LocalTransform>(SystemAPI.GetSingletonEntity<RoterDataComp>());
            roterTrfm.ValueRW.Position.x = 100;
        }
        else
        {
            ECB.RemoveComponent<ItemPlaceableComp>(itemEntity);
            ECB.AddComponent<ItemPlacedComp>(itemEntity);
            
            if(SystemAPI.HasSingleton<ItemActiveEditableComp>()) ECB.RemoveComponent<ItemActiveEditableComp>(SystemAPI.GetSingletonEntity<ItemActiveEditableComp>());
            ECB.AddComponent<ItemActiveEditableComp>(itemEntity);
        }
    }
}