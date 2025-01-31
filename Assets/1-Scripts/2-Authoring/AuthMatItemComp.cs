using UnityEngine;
using Unity.Entities;

public class AuthMatItemComp : MonoBehaviour
{
    public Color defColorVal;

    public class Baker : Baker<AuthMatItemComp>
    {
        public override void Bake(AuthMatItemComp authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new MatItemColorComp() { colorValue = authoring.defColorVal });
            AddComponent(entity, new MatItemOpacityComp() { opacityValue = 1 });
        }
    }
}