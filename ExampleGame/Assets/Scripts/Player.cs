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

	private void Start () {
    controller = GetComponent<Controller2D>();

    gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

  private void Update() {
    Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below) {
      velocity.y = maxJumpVelocity;
    }
    if (Input.GetKeyUp(KeyCode.Space)) {
      if(velocity.y > minJumpVelocity) {
        velocity.y = minJumpVelocity;
      }
    }

    float targetVelocityX = input.x * moveSpeed;
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime, input);

    if (controller.collisions.above || controller.collisions.below) {
      velocity.y = 0;
    }
  }
}
