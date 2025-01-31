using UnityEngine;
using Unity.Entities;

public class AuthItemColorManagerComp : MonoBehaviour
{
    public Color invalidColorVal;

    public class Baker : Baker<AuthItemColorManagerComp>
    {
        public override void Bake(AuthItemColorManagerComp authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ItemColorManagerComp() { invalidColorVal = authoring.invalidColorVal });
        }
    }
}