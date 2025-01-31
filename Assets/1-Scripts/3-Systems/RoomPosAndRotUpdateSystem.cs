using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct RoomPosAndRotUpdateSystem : ISystem
{
	bool isFirstTouch;

	[BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<RoomPosAndRotComp>();
	}
	
	public void OnUpdate(ref SystemState state)
	{
		InputManager input = InputManager.Instance;

		foreach(var _trfm in SystemAPI.Query<RefRW<LocalTransform>>())
		{
			_trfm.ValueRW.Scale = 0.3f;
		}

        if (!input.isFirstTouchPress)
        {
            isFirstTouch = true;
            return;
        }

        if (isFirstTouch) isFirstTouch = false;
        else return;

        Entity room = SystemAPI.GetSingletonEntity<RoomComp>();
		RefRW<LocalTransform> trfm = SystemAPI.GetComponentRW<LocalTransform>(room);
		
		RoomPosAndRotComp posAndRotComp = SystemAPI.GetSingleton<RoomPosAndRotComp>();
		
		trfm.ValueRW.Position = posAndRotComp.pos;
		trfm.ValueRW.Rotation = posAndRotComp.rot;
		
		state.EntityManager.RemoveComponent<RoomPosAndRotComp>(room);
	}
}