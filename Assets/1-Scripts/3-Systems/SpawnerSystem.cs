using Unity.Entities;
using Unity.Burst;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemIndexComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ECB = new(Unity.Collections.Allocator.Temp);

        foreach(SpawnerAspect spawnerAspect in SystemAPI.Query<SpawnerAspect>())
        {
            spawnerAspect.SpawnItem(ECB);
        }
        
        ECB.Playback(state.EntityManager);
    }
}
