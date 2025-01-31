using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Opacity")]
public struct MatItemOpacityComp : IComponentData
{
    public float opacityValue;
}