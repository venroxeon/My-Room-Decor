using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class ItemCompAuth : MonoBehaviour
{
    public float upScale;
    public LayerMask targetLayer;

    public class Baker : Baker<ItemCompAuth>
    {
        public override void Bake(ItemCompAuth authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            uint layer = 1u << (int)math.log2(authoring.targetLayer.value);
            
            AddComponent(entity, new ItemAnimSelectedComp() { upScale = authoring.upScale, targetLayer = layer });
            
            AddComponent(entity, new ItemPlaceableComp());
            AddComponent(entity, new ItemCollidableComp());
        }
    }
}