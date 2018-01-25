﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

  Player player;

  private void Start() {
    player = GetComponent<Player>();
  }

  private void Update() {
    Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    player.SetDirectionalInput(directionalInput);

    if (Input.GetKeyDown(KeyCode.Space)) {
      player.OnJumpInputDown();
    }
    if (Input.GetKeyUp(KeyCode.Space)) {
      player.OnJumpInputUp();
    }
    if (Input.GetMouseButton(0)) {
      player.ShootRed();
    }
    if (Input.GetMouseButton(1)) {
      player.ShootBlue();
    }
    if (Input.GetKeyDown(KeyCode.E)) {
      player.ShootYellow();
    }
  }
}
