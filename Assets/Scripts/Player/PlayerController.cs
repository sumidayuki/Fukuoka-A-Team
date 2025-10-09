using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseUpdate
{
    [Header("=== Player Components ===")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform model;

    [Header("=== Player Settings ===")]
    [SerializeField] private PlayerData playerData;

    private Player player;
    private InputInfo input;

    private void Start()
    {
        GameManager.Instance.RegisterSystem(this);
    }
    private void OnDisable()
    {
        GameManager.Instance.UnregisterSystem(this);
    }

    public override void Enter()
    {
        input = InputManager.Instance.Info;
        player = new Player();
        player.Rb = rb;
        player.Model = model;
        player.Enter(playerData);
    }

    public override void Execute()
    {
        player.Execute(input);

#if ENABLE_INPUT_SYSTEM
        input.Plant = UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        input.Plant = Input.GetKeyDown(KeyCode.Space);
#endif

        player.Execute(input);
    }

    public override void LateExecute()
    {
        player.LateExecute(input);
    }
}
