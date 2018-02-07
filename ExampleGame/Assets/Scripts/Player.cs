﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

  public Gun gun;

  public float minJumpHeight;
  public float maxJumpHeight;
  public float maxJumpDistance;
  public float downArcMultiplier;
  public float moveSpeed;
  public float accelerationTime;

  public Vector2 wallJumpClimb;
  public Vector2 wallJumpOff;
  public Vector2 wallLeap;
  public float wallSlideSpeedMax;
  public float wallStickTime;
  float timeToWallUnstick;
  
  bool applyGravityHeavy = false;
  float gravity;
  float gravityHeavy;
  float maxJumpVelocity;
  float startingJumpHeight;
  Vector3 velocity;

  float velocityXSmoothing;

  Controller2D controller;

  Vector2 directionalInput;
  bool wallSliding;
  int wallDirX;

  private void Start () {
    controller = GetComponent<Controller2D>();

    float xDistanceAtPeak = maxJumpDistance/(1 + downArcMultiplier);
    float xDistanceAtPeakHeavy = xDistanceAtPeak * downArcMultiplier;
    gravity = -(2 * maxJumpHeight * Mathf.Pow(moveSpeed, 2)) / Mathf.Pow(xDistanceAtPeak, 2);
    gravityHeavy = -(2 * maxJumpHeight * Mathf.Pow(moveSpeed, 2)) / Mathf.Pow(xDistanceAtPeakHeavy, 2);
    maxJumpVelocity = (2 * maxJumpHeight * moveSpeed) / xDistanceAtPeak;
  }

  private void Update() {
    CalculateVelocity();
    HandleWallSliding();

    controller.Move(velocity * Time.deltaTime, directionalInput);

    if (controller.collisions.below) {
      applyGravityHeavy = false;
    }

    if (controller.collisions.above || controller.collisions.below) {
      if (controller.collisions.slidingDownMaxSlope) {
        velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
      }
      else {
        velocity.y = 0;
      }
    }
  }

  private void CalculateVelocity() {
    float targetVelocityX = directionalInput.x * moveSpeed;
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);

    if (!controller.collisions.slidingDownMaxSlope && velocity.y <= 0) {
      applyGravityHeavy = true;
    }
    if (applyGravityHeavy) {
      velocity.y += gravityHeavy * Time.deltaTime;
    }
    else {
      velocity.y += gravity * Time.deltaTime;
    }
  }

  private void HandleWallSliding() {
    wallSliding = false;
    wallDirX = (controller.collisions.left) ? -1 : 1;
    if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
      wallSliding = true;
      if (velocity.y < -wallSlideSpeedMax) {
        velocity.y = -wallSlideSpeedMax;
      }
      if (timeToWallUnstick > 0) {
        velocityXSmoothing = 0;
        velocity.x = 0;
        if (directionalInput.x != wallDirX && directionalInput.x != 0) {
          timeToWallUnstick -= Time.deltaTime;
        }
        else {
          timeToWallUnstick = wallStickTime;
        }
      }
      else {
        timeToWallUnstick = wallStickTime;
      }
    }
  }

  public void SetDirectionalInput(Vector2 input) {
    directionalInput = input;
  }

  public void OnJumpInputDown() {
    startingJumpHeight = transform.position.y;
    if (wallSliding) {
      if (wallDirX == directionalInput.x) {
        velocity.x = -wallDirX * wallJumpClimb.x;
        velocity.y = wallJumpClimb.y;
      }
      else if (directionalInput.x == 0) {
        velocity.x = -wallDirX * wallJumpOff.x;
        velocity.y = wallJumpOff.y;
      }
      else {
        velocity.x = -wallDirX * wallLeap.x;
        velocity.y = wallLeap.y;
      }
    }
    if (controller.collisions.below) {
      if (controller.collisions.slidingDownMaxSlope) {
        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
      }
      else {
        velocity.y = maxJumpVelocity;
      }
    }
  }

  public void OnJumpInputUp() {
    float jumpHeightDifference = transform.position.y - startingJumpHeight;
    float distanceToMinJumpHeight = minJumpHeight - jumpHeightDifference;
    if (distanceToMinJumpHeight > 0) {
      float velocityToMinHeight = Mathf.Sqrt(2 * Mathf.Abs(gravity) * distanceToMinJumpHeight);
      if (velocity.y > velocityToMinHeight) {
        velocity.y = velocityToMinHeight;
      }
    }
    else {
      applyGravityHeavy = true;
    }
  }

  public void ShootRed() {
    gun.redTrigger = true;
  }

  public void StopRed() {
    gun.redTrigger = false;
  }

  public void ShootBlue() {
    gun.blueTrigger = true;
  }

  public void StopBlue() {
    gun.blueTrigger = false;
  }

  public void ShootYellow() {
    gun.yellowTrigger = true;
  }
}
