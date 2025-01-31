using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

[System.Serializable]
public struct ItemList
{
    public List<GameObject> listItem;
}

public class AuthItemManagerComp : MonoBehaviour
{
    public List<ItemList> listStructItems = new();

    public class Baker : Baker<AuthItemManagerComp>
    {
        public override void Bake(AuthItemManagerComp authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            DynamicBuffer<ItemEntityBuffElem> itemEntityBuffer = AddBuffer<ItemEntityBuffElem>(entity);
            DynamicBuffer<ItemIndexBuffElem> itemIndexBuffer = AddBuffer<ItemIndexBuffElem>(entity);

            int _index = 0;

            itemIndexBuffer.Add(new ItemIndexBuffElem() { index = _index });

            foreach (ItemList itemList in authoring.listStructItems)
            {
                foreach (GameObject obj in itemList.listItem)
                {
                    itemEntityBuffer.Add(new ItemEntityBuffElem() { itemEntity = GetEntity(obj, TransformUsageFlags.Dynamic) });
                }

                _index += itemList.listItem.Count;

                itemIndexBuffer.Add(new ItemIndexBuffElem() { index = _index });
            }

            AddComponent(entity, new ItemSpawnerComp());
        }
    }
}