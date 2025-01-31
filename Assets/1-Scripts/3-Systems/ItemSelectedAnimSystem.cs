using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct ItemSelectedAnimSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<ItemPlaceableComp>();
	}
	
	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		foreach(var (itemAnimComp, trfm) in SystemAPI.Query<RefRW<ItemAnimSelectedComp>, RefRW<LocalTransform>>().WithAll<ItemPlaceableComp>())
		{
			if(itemAnimComp.ValueRO.isFloored)
			{
	            if (itemAnimComp.ValueRO.isScaled)
	            {
	                trfm.ValueRW.Scale -= itemAnimComp.ValueRO.upScale;
	                
	                itemAnimComp.ValueRW.isScaled = false;
	            }
			}
			else
			{
				if (!itemAnimComp.ValueRO.isScaled)
				{
	                trfm.ValueRW.Scale += itemAnimComp.ValueRO.upScale;
	                
	                itemAnimComp.ValueRW.isScaled = true;
				}
			}
		}
	}
}