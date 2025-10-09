using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public InputInfo Info { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Info = new InputInfo();
    }

    private void Update()
    {
        InputClear();
    }

    public void SwitchActionMap(string mapName)
    {
        gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap(mapName);
    }

    public void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        Info.Move = new Vector3(input.x, 0, input.y);
    }

    public void OnLook(InputValue value)
    {
        var input = value.Get<Vector2>();
        Info.Look = input;
    }

    public void OnLeftClick()
    {
        Info.LeftClick = true;
    }

    public void OnRightClick(InputValue value)
    {
        var input = value.Get<float>();
        Info.RightClick = input > 0 ? true : false;
    }

    public void OnPlant()
    {
        Info.Plant = true;
    }

    public void OnPause()
    {
        Info.Pause = !Info.Pause;
    }

    private void InputClear()
    {
        Info.LeftClick = false;
        Info.Plant = false;
    }
}
