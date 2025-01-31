using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public readonly partial struct SpawnerAspect : IAspect
{
    readonly Entity itemManagerEntity;
    readonly RefRO<ItemIndexComp> itemIndexComp;

    readonly DynamicBuffer<ItemEntityBuffElem> itemBuffer;
    readonly DynamicBuffer<ItemIndexBuffElem> itemIndexBuffer;

    public void SpawnItem(in EntityCommandBuffer ECB)
    {
        ECB.Instantiate(itemBuffer[itemIndexBuffer[itemIndexComp.ValueRO.row].index + itemIndexComp.ValueRO.col].itemEntity);

        ECB.RemoveComponent<ItemIndexComp>(itemManagerEntity);
    }
}