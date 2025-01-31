using Unity.Entities;
using UnityEngine;

public class AuthRoomComp : MonoBehaviour
{
	public class Baker : Baker<AuthRoomComp>
	{
		public override void Bake(AuthRoomComp auth)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			
			AddComponent<RoomComp>(entity);
		}
	}
}