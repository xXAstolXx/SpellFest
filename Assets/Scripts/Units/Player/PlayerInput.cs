using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions playerInputBindings;
    private PlayerMovement movement;
    private Player player;
    private PlayerGraphic graphic;

    private InputAction wasd;
    private InputAction space;

    private InputAction mouseLeft;
    private InputAction mouseRight;

    private InputAction interactKey;

    private InputAction escapeKey;

    float mouseWheelValue;
    const float mouseWheelThreshold = 1;

    bool isPauseMenuActive = false;


    private void Awake()
    {
        playerInputBindings = new PlayerInputActions();
        
        movement = GetComponent<PlayerMovement>();
        player = GetComponent<Player>();
        graphic = GetComponentInChildren<PlayerGraphic>();

        wasd = playerInputBindings.KeyboardMouse.WASD;
        //wasd.canceled += EndMove;

        space = playerInputBindings.KeyboardMouse.Space;
        space.started += SpacePressed;

        mouseLeft = playerInputBindings.KeyboardMouse.MouseLeft;
        mouseLeft.started += MouseLeftClicked;
        mouseLeft.canceled += MouseLeftReleased;

        mouseRight = playerInputBindings.KeyboardMouse.MouseRight;
        mouseRight.started += MouseRightClicked;

        interactKey = playerInputBindings.KeyboardMouse.InteractKey;
        interactKey.started += InteractKeyPressed;

        escapeKey = playerInputBindings.KeyboardMouse.Escape;
        escapeKey.started += EscapeKeyPressed;
    }

    private void Start()
    {
        UI.Instance.pauseMenu.OnResume.AddListener(ActivateLeftMouseClick);
    }

    private void OnEnable()
    {
        wasd.Enable();
        mouseLeft.Enable();
        mouseRight.Enable();
        space.Enable();
        interactKey.Enable();
        escapeKey.Enable();
    }

    private void OnDisable()
    {
        wasd.Disable();
        mouseLeft.Disable();
        mouseRight.Disable();
        space.Disable();
        interactKey.Disable();
        escapeKey.Disable();
    }

    private void Update()
    {
        //float value = Mouse.current.scroll.ReadValue().normalized.y;
        //if (value > 0)
        //{
        //    if(mouseWheelValue < 0)
        //    {
        //        mouseWheelValue = 0;
        //    }
        //    mouseWheelValue += value;
        //    if(mouseWheelValue >= mouseWheelThreshold)
        //    {
        //        mouseWheelValue = 0;
        //        player.UpdateActiveSpell(-1);
        //    }
        //}
        //if (value < 0)
        //{
        //    if (mouseWheelValue > 0)
        //    {
        //        mouseWheelValue = 0;
        //    }
        //    mouseWheelValue += value;
        //    if (mouseWheelValue <= -mouseWheelThreshold)
        //    {
        //        mouseWheelValue = 0;
        //        player.UpdateActiveSpell(1);
        //    }
        //}
    }

    private void MouseLeftClicked(CallbackContext context)
    {
        //Debug.Log("left clicked");
        player.LeftClick(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void MouseRightClicked(CallbackContext context)
    {
        player.RightClick(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void MouseLeftReleased(CallbackContext context)
    {
        player.LeftClickReleased(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void UpdateMoveInput(CallbackContext context)
    {
        if(wasd.enabled)
        {
            Vector2 moveVector = context.ReadValue<Vector2>().normalized;
            player.UpdateMoveInput(moveVector);
        }
    }

    private void SpacePressed(CallbackContext context)
    {
        player.ChangeSpell();
    }

    private void InteractKeyPressed(CallbackContext context)
    {
        player.Interact();
    }

    private void EscapeKeyPressed(CallbackContext context)
    {
        if(isPauseMenuActive != true)
        { 
            UI.Instance.pauseMenu.ShowPopUp();
            UI.Instance.ActivateOverlay();
            DeactivateLeftMouseClick();
            isPauseMenuActive = true;
        }
        else
        {
            UI.Instance.pauseMenu.ClosePopUp();
            ActivateLeftMouseClick();
            isPauseMenuActive = false;
        }
    }



    private void ActivateLeftMouseClick()
    {
        mouseLeft.Enable();
        UI.Instance.DeActivateOverlay();
    }

    private void DeactivateLeftMouseClick()
    {
        mouseLeft.Disable();
    }

    public void EnableInput()
    {
        wasd.Enable();
        mouseLeft.Enable();
        mouseRight.Enable();
        space.Enable();
        interactKey.Enable();
        escapeKey.Enable();
    }

    public void DisableInput()
    {
        wasd.Disable();
        mouseLeft.Disable();
        mouseRight.Disable();
        space.Disable();
        interactKey.Disable();
        escapeKey.Disable();
    }
}
