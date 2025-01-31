using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Unity.Physics;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [HideInInspector] public bool isFirstTouchPress;

    TouchInput touchInput;

    void OnDisable()
    {
        touchInput.Disable();

        touchInput.TouchInputs.FirstTouchPress.started -= ctx => isFirstTouchPress = true;
        touchInput.TouchInputs.FirstTouchPress.canceled -= ctx => isFirstTouchPress = false;
    }

    void OnEnable()
    {
        touchInput = new();
        touchInput.Enable();

        touchInput.TouchInputs.FirstTouchPress.started += ctx => isFirstTouchPress = true;
        touchInput.TouchInputs.FirstTouchPress.canceled += ctx => isFirstTouchPress = false;
    }

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public float3 GetFirstTouchPos(float distFromCam)
    {
        float2 screenPos = touchInput.TouchInputs.FirstTouchPos.ReadValue<Vector2>();
        float3 touchPos = new() { x = screenPos.x, y = screenPos.y, z = distFromCam };
        return Camera.main.ScreenToWorldPoint(touchPos);
    }

    public float3 GetFirstScreenTouchPos()
    {
        float2 screenPos = touchInput.TouchInputs.FirstTouchPos.ReadValue<Vector2>();
        float3 touchPos = new() { x = screenPos.x, y = screenPos.y, z = 0 };

        return touchPos;
    }

    public Unity.Physics.Ray GetRay()
    {
        float3 val = Camera.main.ScreenToWorldPoint(touchInput.TouchInputs.FirstTouchPos.ReadValue<Vector2>());
        val.z = 0;

        float3 delta = (val - (float3)Camera.main.transform.position) * 10;

        return new Unity.Physics.Ray() { Origin = delta, Displacement = delta };
    }

    public void ACT_FirstTouchPressStarted(Action<InputAction.CallbackContext> action_started)
    {
        touchInput.TouchInputs.FirstTouchPress.started += action_started;
    }
    public void RCT_FirstTouchPressStarted(Action<InputAction.CallbackContext> action_started)
    {
        touchInput.TouchInputs.FirstTouchPress.started -= action_started;
    }

    public void ACT_FirstTouchPressCancelled(Action<InputAction.CallbackContext> action_cancelled)
    {
        touchInput.TouchInputs.FirstTouchPress.canceled += action_cancelled;
    }

    public void RCT_FirstTouchPressCancelled(Action<InputAction.CallbackContext> action_cancelled)
    {
        touchInput.TouchInputs.FirstTouchPress.canceled -= action_cancelled;
    }
}