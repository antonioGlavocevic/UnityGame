﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

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

  void Start () {
    controller = GetComponent<Controller2D>();

    float xDistanceAtPeak = maxJumpDistance/(1 + downArcMultiplier);
    float xDistanceAtPeakHeavy = xDistanceAtPeak * downArcMultiplier;
    gravity = -(2 * maxJumpHeight * moveSpeed * moveSpeed) / (xDistanceAtPeak * xDistanceAtPeak);
    gravityHeavy = -(2 * maxJumpHeight * moveSpeed * moveSpeed) / (xDistanceAtPeakHeavy * xDistanceAtPeakHeavy);
    maxJumpVelocity = (2 * maxJumpHeight * moveSpeed) / xDistanceAtPeak;
  }

  void Update() {
    HandleWallSliding();
    Vector2 verletIntegration = new Vector2(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime + 0.5f * gravity * Time.deltaTime * Time.deltaTime);
    controller.Move(verletIntegration, directionalInput);
    CalculateVelocity();
    PostCollisionHandler();
  }

  void HandleWallSliding() {
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

  void CalculateVelocity() {
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

  void PostCollisionHandler() {
    if (controller.collisions.above || controller.collisions.below) {
      if (controller.collisions.slidingDownMaxSlope) {
        velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
      }
      else {
        applyGravityHeavy = false;
        velocity.y = 0;
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
}
