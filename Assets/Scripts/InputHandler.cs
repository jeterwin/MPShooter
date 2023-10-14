using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputActions inputActions;
    private Vector2 mouseInput;
    private Vector2 movementInput;
    private int nextGun = 1;
    private bool pressedJump = false;
    private bool pressedShoot = false;
    private bool pressedReload = false;
    public int NextGun
    {
        get { return nextGun - 1; }
    }
    public bool PressedShoot
    {
        get { return pressedShoot; }
    }
    public bool PressedReload
    {
        get { return pressedReload; }
    }
    public bool PressedJump
    {
        get { return pressedJump; }
    }
    public Vector2 MouseInput
    {
        get { return mouseInput; }
    }
    public Vector2 MovementDir
    {
        get { return movementInput; }
    }
    private void Awake()
    {
        inputActions = new InputActions();

        inputActions.Movement.Move.started += move;
        inputActions.Movement.Move.canceled += move;
        inputActions.Movement.Move.performed += move;

        inputActions.Movement.Jump.started += jump;
        inputActions.Movement.Jump.canceled += jump;
        inputActions.Movement.Jump.performed += jump;

        inputActions.Movement.Shoot.started += shoot;
        inputActions.Movement.Shoot.canceled += shoot;
        inputActions.Movement.Shoot.performed += shoot;

        inputActions.Movement.Look.started += look;
        inputActions.Movement.Look.canceled += look;
        inputActions.Movement.Look.performed += look;

        inputActions.Movement.Reload.started += reload;
        inputActions.Movement.Reload.canceled += reload;
        inputActions.Movement.Reload.performed += reload;

        inputActions.Movement.SwitchWeapon.started += switchWeapon;
        inputActions.Movement.SwitchWeapon.canceled += switchWeapon;
        inputActions.Movement.SwitchWeapon.performed += switchWeapon;
    }
    void switchWeapon(InputAction.CallbackContext ctx)
    {
        nextGun = int.Parse(ctx.control.name);
    }
    void reload(InputAction.CallbackContext ctx)
    {
        pressedReload = ctx.ReadValueAsButton();
    }
    void look(InputAction.CallbackContext ctx)
    {
        mouseInput = ctx.ReadValue<Vector2>() * Time.smoothDeltaTime;
    }
    void shoot(InputAction.CallbackContext ctx)
    {
        pressedShoot = ctx.ReadValueAsButton();
    }
    void jump(InputAction.CallbackContext ctx)
    {
        pressedJump = ctx.ReadValueAsButton();
    }

    void move(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        inputActions.Movement.Enable();
    }
    private void OnDisable()
    {
        inputActions.Movement.Disable();
    }

}
