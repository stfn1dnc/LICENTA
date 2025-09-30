using UnityEngine;
using UnityEngine.InputSystem;

public class EnableUIActions : MonoBehaviour
{
    public InputActionAsset inputActions;

    void OnEnable()
    {
        inputActions.FindActionMap("UI").Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap("UI").Disable();
    }
}
