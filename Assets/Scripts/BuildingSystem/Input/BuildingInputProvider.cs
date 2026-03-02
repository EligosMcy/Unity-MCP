using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingInputProvider : MonoBehaviour
{
    public InputActionReference MousePositionActionReference;
    public InputActionReference MouseClickActionReference;
    public InputActionReference ModeSwitchActionReference;

    public event Action<Vector2> OnMousePositionChanged;
    public event Action OnMouseClicked;
    public event Action OnModeSwitched;

    private void OnEnable()
    {
        if (MousePositionActionReference != null && MousePositionActionReference.action != null)
        {
            MousePositionActionReference.action.performed += OnMousePositionPerformed;
            MousePositionActionReference.action.Enable();
        }

        if (MouseClickActionReference != null && MouseClickActionReference.action != null)
        {
            MouseClickActionReference.action.performed += OnMouseClickPerformed;
            MouseClickActionReference.action.Enable();
        }

        if (ModeSwitchActionReference != null && ModeSwitchActionReference.action != null)
        {
            ModeSwitchActionReference.action.performed += OnModeSwitchPerformed;
            ModeSwitchActionReference.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (MousePositionActionReference != null && MousePositionActionReference.action != null)
        {
            MousePositionActionReference.action.performed -= OnMousePositionPerformed;
            MousePositionActionReference.action.Disable();
        }

        if (MouseClickActionReference != null && MouseClickActionReference.action != null)
        {
            MouseClickActionReference.action.performed -= OnMouseClickPerformed;
            MouseClickActionReference.action.Disable();
        }

        if (ModeSwitchActionReference != null && ModeSwitchActionReference.action != null)
        {
            ModeSwitchActionReference.action.performed -= OnModeSwitchPerformed;
            ModeSwitchActionReference.action.Disable();
        }
    }

    private void OnMousePositionPerformed(InputAction.CallbackContext context)
    {
        OnMousePositionChanged?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnMouseClickPerformed(InputAction.CallbackContext context)
    {
        OnMouseClicked?.Invoke();
    }

    private void OnModeSwitchPerformed(InputAction.CallbackContext context)
    {
        OnModeSwitched?.Invoke();
    }

    public Vector2 GetMousePosition()
    {
        if (MousePositionActionReference != null && MousePositionActionReference.action != null)
        {
            return MousePositionActionReference.action.ReadValue<Vector2>();
        }
        return Vector2.zero;
    }

    public bool WasMouseClickedThisFrame()
    {
        if (MouseClickActionReference != null && MouseClickActionReference.action != null)
        {
            return MouseClickActionReference.action.WasPressedThisFrame();
        }
        return false;
    }
}
