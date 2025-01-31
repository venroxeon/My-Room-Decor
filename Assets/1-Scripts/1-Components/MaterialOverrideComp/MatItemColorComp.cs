using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[MaterialProperty("_Color")]
public struct MatItemColorComp : IComponentData
{
    public Color colorValue;
}