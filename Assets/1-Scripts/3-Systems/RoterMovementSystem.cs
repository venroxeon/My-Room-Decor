using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct RoterMovementSystem : ISystem
{
	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<ItemPlaceableComp>();
		state.RequireForUpdate<RoterDataComp>();
	}
	
	public void OnUpdate(ref SystemState state)
	{
		RefRW<LocalTransform> trfm = SystemAPI.GetComponentRW<LocalTransform>(SystemAPI.GetSingletonEntity<RoterDataComp>());

		trfm.ValueRW.Position = InputManager.Instance.GetFirstTouchPos(1);
		trfm.ValueRW.Rotation = Camera.main.transform.rotation;
	}
}