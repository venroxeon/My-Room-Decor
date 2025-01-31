using Unity.Entities;
using UnityEngine;

public struct ItemColorManagerComp : IComponentData
{
    public Color defColorVal, invalidColorVal;
}