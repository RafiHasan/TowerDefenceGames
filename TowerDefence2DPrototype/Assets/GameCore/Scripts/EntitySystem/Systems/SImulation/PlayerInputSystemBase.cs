using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(FixedTickSystemGroup))]
public partial class PlayerInputSystemBase : SystemBase
{
    private InputActions _inputActions;
    protected override void OnStartRunning()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();
        RegisterInput();
    }

    protected override void OnStopRunning()
    {
        _inputActions.Disable();
        UnregisterInput();
    }

    private void RegisterInput()
    {
        _inputActions.Player.StartGame.performed += StartGame_performed;
        _inputActions.Player.ScreenPosition.performed += ScreenPosition_performed;
        _inputActions.Player.Select.performed += Select_performed;
        _inputActions.Player.DeSelect.performed += DeSelect_performed;
        _inputActions.Player.SpawnItem1.performed += SpawnItem1_performed;
        _inputActions.Player.SpawnItem2.performed += SpawnItem2_performed;
        _inputActions.Player.SpawnItem3.performed += SpawnItem3_performed;
        _inputActions.Player.SpawnItem4.performed += SpawnItem4_performed;
        _inputActions.Player.SpawnItem5.performed += SpawnItem5_performed;
        _inputActions.Player.SpawnItem6.performed += SpawnItem6_performed;
        _inputActions.Player.Reset.performed += Reset_performed;
        _inputActions.Player.Save.performed += Save_performed;
        _inputActions.Player.Load.performed += Load_performed;
        _inputActions.Player.Delete.performed += Delete_performed;


    }

    

    private void StartGame_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.GameStart = true;
    }
    
    private void ScreenPosition_performed(InputAction.CallbackContext obj)
    {
        Vector3 clickPosition = obj.ReadValue<Vector2>();

        if (!SystemAPI.HasSingleton<InputComponent>())
            return;

        

        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();

        if (inputComponent.ValueRO.Spawn)
            return;


            clickPosition.z = Camera.main.nearClipPlane;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        inputComponent.ValueRW.WorldPosition = worldPosition;
    }
    private void Select_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Selected = true;
        inputComponent.ValueRW.DeSelected = false;

    }
    private void DeSelect_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.DeSelected = true;
        inputComponent.ValueRW.Selected = false;
    }
    private void SpawnItem1_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Spawn = true;
        inputComponent.ValueRW.ItemIndex = 1;
        inputComponent.ValueRW.Selected = false;
    }
    private void SpawnItem2_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Spawn = true;
        inputComponent.ValueRW.ItemIndex = 2;
        inputComponent.ValueRW.Selected = false;
    }
    private void SpawnItem3_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Spawn = true;
        inputComponent.ValueRW.ItemIndex = 3;
        inputComponent.ValueRW.Selected = false;
    }
    private void SpawnItem4_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Spawn = true;
        inputComponent.ValueRW.ItemIndex = 4;
        inputComponent.ValueRW.Selected = false;
    }
    private void SpawnItem5_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Spawn = true;
        inputComponent.ValueRW.ItemIndex = 5;
        inputComponent.ValueRW.Selected = false;
    }

    private void SpawnItem6_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Spawn = true;
        inputComponent.ValueRW.ItemIndex = 6;
        inputComponent.ValueRW.Selected = false;
    }
    private void Reset_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Reset = true;
    }

    private void Load_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Load = true;
    }

    private void Save_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Save = true;
    }

    private void Delete_performed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;
        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();
        inputComponent.ValueRW.Delete = true;
        inputComponent.ValueRW.Selected = false;
    }

    private void UnregisterInput()
    {
        _inputActions.Player.StartGame.performed -= StartGame_performed;
        _inputActions.Player.ScreenPosition.performed -= ScreenPosition_performed;
        _inputActions.Player.Select.performed -= Select_performed;
        _inputActions.Player.DeSelect.performed -= DeSelect_performed;
        _inputActions.Player.SpawnItem1.performed -= SpawnItem1_performed;
        _inputActions.Player.SpawnItem2.performed -= SpawnItem2_performed;
        _inputActions.Player.SpawnItem3.performed -= SpawnItem3_performed;
        _inputActions.Player.SpawnItem4.performed -= SpawnItem4_performed;
        _inputActions.Player.SpawnItem5.performed -= SpawnItem5_performed;
        _inputActions.Player.SpawnItem6.performed -= SpawnItem6_performed;
        _inputActions.Player.Reset.performed -= Reset_performed;
        _inputActions.Player.Save.performed -= Save_performed;
        _inputActions.Player.Load.performed -= Load_performed;
        _inputActions.Player.Delete.performed -= Delete_performed;
    }

    protected override void OnUpdate()
    {
        
    }
}
