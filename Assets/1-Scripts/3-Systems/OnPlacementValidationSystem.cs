//using Unity.Entities;
//using Unity.Burst;

//[UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
//[BurstCompile]
//public partial struct OnPlacementValidationSystem : ISystem
//{
//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        state.RequireForUpdate<ItemPlaceableComp>();
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {
//        InputSystemManager input = InputSystemManager.Instance;

//        if (input.isFirstTouchPress) return;

//        EntityCommandBuffer ECB = SystemAPI.GetSingleton<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);



//        foreach (var (collComp, entity) in SystemAPI.Query<ItemAnimSelectedComp>().WithAll<PlacedComp>().WithEntityAccess())
//        {
//            if (!collComp.isOverlapping) continue;

//            ECB.DestroyEntity(collComp.roterEntity);
//            ECB.DestroyEntity(entity);
//        }
//    }
//}
