using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseUpdate
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform model;

    private Player player;
    private InputInfo input = new InputInfo();

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterSystem(this);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterSystem(this);
    }

    public override void Enter()
    {
        player = new Player();
        player.Rb = rb;
        player.Model = model;
        player.Enter();
    }

    public override void Execute()
    {
        input.Move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        player.Execute(input);
    }
}
