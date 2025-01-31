using Unity.Entities;
using Unity.Burst;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[BurstCompile]
public partial struct CollisionMaterialReplacementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ItemCollidableComp>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {   
        ItemColorManagerComp itemColorManager = SystemAPI.GetSingleton<ItemColorManagerComp>();
        
        foreach(var (itemAnimSelectedComp, itemMatColorComp, itemOpacityComp) in SystemAPI.Query<RefRW<ItemAnimSelectedComp>, RefRW<MatItemColorComp>, RefRW<MatItemOpacityComp>>())
        {
            if (itemAnimSelectedComp.ValueRO.isOverlapping)
            {
                if(!itemAnimSelectedComp.ValueRO._isOverlapping) 
                {
                    itemOpacityComp.ValueRW.opacityValue = 0.4f;
                    itemMatColorComp.ValueRW.colorValue = itemColorManager.invalidColorVal;

                    itemAnimSelectedComp.ValueRW._isOverlapping = true;
                    itemAnimSelectedComp.ValueRW.isReverted = false;
                }
            }
            else
            {
                if (!itemAnimSelectedComp.ValueRO.isReverted)
                {
                    itemOpacityComp.ValueRW.opacityValue = 1;
                    itemMatColorComp.ValueRW.colorValue = UnityEngine.Color.white;

                    itemAnimSelectedComp.ValueRW.isReverted = true;
                    itemAnimSelectedComp.ValueRW._isOverlapping = false;
                }
            }

            itemAnimSelectedComp.ValueRW.isOverlapping = false;
        }
    }
}
