using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseUpdate
{
    [Header("=== Player Components ===")]
    [SerializeField] private Rigidbody rb;

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
        player.Rb = GetComponent<Rigidbody>();
        player.PlayerTransform = this.gameObject.transform;
        player.Enter(playerData);
    }

    public override void Execute()
    {
        player.Execute(input);
    }

    public override void FixedExecute()
    {
        player.FixedExecute(input);
    }

    public override void LateExecute()
    {
        player.LateExecute(input);
    }
}
