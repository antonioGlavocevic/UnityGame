using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

  public float maxJumpHeight = 4;
  public float minJumpHeight = 1;
  public float timeToJumpApex = 0.4f;
  public float moveSpeed = 6;
  public float accelerationTimeAirborne = .2f;
  public float accelerationTimeGrounded = .1f;

  float gravity;
  float maxJumpVelocity;
  float minJumpVelocity;
  Vector3 velocity;

  float velocityXSmoothing;

  Controller2D controller;

  Vector2 directionalInput;

	private void Start () {
    controller = GetComponent<Controller2D>();

    gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

  private void Update() {
    CalculateVelocity();

    controller.Move(velocity * Time.deltaTime, directionalInput);

    if (controller.collisions.above || controller.collisions.below) {
      velocity.y = 0;
    }
  }

  private void CalculateVelocity() {
    float targetVelocityX = directionalInput.x * moveSpeed;
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
    velocity.y += gravity * Time.deltaTime;
  }

  public void SetDirectionalInput(Vector2 input) {
    directionalInput = input;
  }

  public void OnJumpInputDown() {
    if (controller.collisions.below) {
      velocity.y = maxJumpVelocity;
    }
  }

  public void OnJumpInputUp() {
    if (velocity.y > minJumpVelocity) {
      velocity.y = minJumpVelocity;
    }
  }
}
