using Unity.Entities;
using Unity.Physics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
public partial struct CollisionDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemCollidableComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        JobHandle handle = new ResetItemOverlapStatus().ScheduleParallel(state.Dependency);
        handle.Complete();
        
        state.Dependency = new PlaceableCollisionJob
        {
            ItemAnimSelectedCompLookUp = SystemAPI.GetComponentLookup<ItemAnimSelectedComp>(),
            collidableCompLookUp = SystemAPI.GetComponentLookup<ItemCollidableComp>(true)

        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [WithAll(typeof(ItemCollidableComp))]
    [BurstCompile]
    public partial struct ResetItemOverlapStatus : IJobEntity
    {
        [BurstCompile]
        public void Execute(ref ItemAnimSelectedComp ItemAnimSelectedComp)
        {
            ItemAnimSelectedComp.isOverlapping = false;
        }
    }

    [BurstCompile]
    public partial struct PlaceableCollisionJob : ITriggerEventsJob
    {
        public ComponentLookup<ItemAnimSelectedComp> ItemAnimSelectedCompLookUp;
        [ReadOnly] public ComponentLookup<ItemCollidableComp> collidableCompLookUp;

        [BurstCompile]
        public void Execute(TriggerEvent trigger)
        {
            if(collidableCompLookUp.HasComponent(trigger.EntityA))
                ItemAnimSelectedCompLookUp.GetRefRW(trigger.EntityA).ValueRW.isOverlapping = true;
            if(collidableCompLookUp.HasComponent(trigger.EntityB))
                ItemAnimSelectedCompLookUp.GetRefRW(trigger.EntityB).ValueRW.isOverlapping = true;
        }
    }
}
