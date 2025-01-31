using UnityEngine;
using Unity.Entities;

public class AuthRoterComp : MonoBehaviour
{
    public class Baker : Baker<AuthRoterComp>
    {
        public override void Bake(AuthRoterComp authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new RoterDataComp());
        }
    }
}
