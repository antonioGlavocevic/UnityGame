using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
[RequireComponent (typeof (GunController))]
public class Player : MonoBehaviour {

  public float maxJumpHeight = 4f;
  public float minJumpHeight = 1f;
  public float timeToJumpApex = 0.4f;
  public float accelerationTimeAirborne = .2f;
  public float accelerationTimeGrounded = .1f;
  public float moveSpeed = 6f;

  public Vector2 wallJumpClimb;
  public Vector2 wallJumpOff;
  public Vector2 wallLeap;
  public float wallSlideSpeedMax = 3f;
  public float wallStickTime = 0.25f;
  float timeToWallUnstick;

  float gravity;
  float maxJumpVelocity;
  float minJumpVelocity;
  Vector3 velocity;

  float velocityXSmoothing;

  Controller2D controller;
  GunController gunController;

  Vector2 directionalInput;
  bool wallSliding;
  int wallDirX;

  private void Start () {
    controller = GetComponent<Controller2D>();
    gunController = GetComponent<GunController>();

    gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
  }

  private void Update() {
    CalculateVelocity();
    HandleWallSliding();

    controller.Move(velocity * Time.deltaTime, directionalInput);

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
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
    velocity.y += gravity * Time.deltaTime;
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
    if (velocity.y > minJumpVelocity) {
      velocity.y = minJumpVelocity;
    }
  }

  public void ShootRed() {
    gunController.ShootRed();
  }

  public void ShootBlue() {
    gunController.ShootBlue();
  }

  public void ShootYellow() {
    gunController.ShootYellow();
  }
}
