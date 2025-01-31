using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup), OrderLast = true)]
[BurstCompile]
public partial struct RoterDisableSystem : ISystem
{
	bool isFirstTouch;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<RoterDataComp>();
	}
	
	public void OnUpdate(ref SystemState state)
	{
		InputManager input = InputManager.Instance;
        if (!input.isFirstTouchPress)
        {
            isFirstTouch = true;
            return;
        }

        if (isFirstTouch) isFirstTouch = false;
        else return;

        RefRW<RoterDataComp> roterDataComp = SystemAPI.GetSingletonRW<RoterDataComp>();

        if (input.isFirstTouchPress)
		{
			if(roterDataComp.ValueRW.isAvailable)
			{
				RefRW<LocalTransform> roterTrfm = SystemAPI.GetComponentRW<LocalTransform>(SystemAPI.GetSingletonEntity<RoterDataComp>());

				roterTrfm.ValueRW.Position.x = 100;
			}
		}

		roterDataComp.ValueRW.isAvailable = true;
    }
}