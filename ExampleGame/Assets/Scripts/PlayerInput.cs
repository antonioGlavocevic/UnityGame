using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

  public Gun gun;

  Player player;

  void Start() {
    player = GetComponent<Player>();
  }

  void Update() {
    Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    player.SetDirectionalInput(directionalInput);

    if (Input.GetKeyDown(KeyCode.Space)) {
      player.OnJumpInputDown();
    }
    if (Input.GetKeyUp(KeyCode.Space)) {
      player.OnJumpInputUp();
    }
    if (Input.GetMouseButton(0)) {
      gun.Shoot();
    }
    if (Input.GetKeyDown(KeyCode.Q)) {
      gun.redTrigger = !gun.redTrigger;
    }
    if (Input.GetKeyDown(KeyCode.E)) {
      gun.blueTrigger = !gun.blueTrigger;
    }
    if (Input.GetKeyDown(KeyCode.R)) {
      gun.yellowTrigger = !gun.yellowTrigger;
    }
  }
}
