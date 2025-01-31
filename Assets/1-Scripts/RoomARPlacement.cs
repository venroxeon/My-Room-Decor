using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;

[RequireComponent(typeof(ARRaycastManager))]
public class RoomARPlacement : PressInputBase
{
    [SerializeField] GameObject room;

    bool m_Pressed;
    EntityManager EM;
    
    protected override void Awake()
    {
        base.Awake();
        m_RaycastManager = GetComponent<ARRaycastManager>();
        
        EM = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void Update()
    {
        if (Pointer.current == null || m_Pressed == false)
            return;

        var touchPosition = Pointer.current.position.ReadValue();

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPos = s_Hits[0].pose;
            
            room.SetActive(true);

            Entity roomEntity = EM.CreateEntityQuery(typeof(RoomComp)).GetSingletonEntity();
            
            EM.AddComponentData(roomEntity, new RoomPosAndRotComp(){ pos = hitPos.position, rot = hitPos.rotation });
        }
    }

    protected override void OnPress(Vector3 position) => m_Pressed = true;

    protected override void OnPressCancel() => m_Pressed = false;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}
