using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Enemy : MonoBehaviour, IBulletInteractable {

  public float startingHealth;
  public float moveSpeed;

  private Controller2D controller;

  private float health;
  private float redHitPerSecond = 4f;
  private float timeSinceHitByRed = 0;
  private float blueHitLinger = 2f;
  private float timeSinceHitByBlue;
  private float yellowHitForce = 20f;
  private Vector2 velocity;
  float velocityXSmoothing;
  public float accelerationTimeGrounded = .1f;
  private float gravity = -50;

  private void Start() {
    controller = GetComponent<Controller2D>();
    health = startingHealth;
    timeSinceHitByBlue = blueHitLinger;
  }

  private void Update() {
    float moveDistance = moveSpeed * Time.deltaTime;
    CalculateVelocity();
    controller.Move(velocity * Time.deltaTime);
  }

  private void CalculateVelocity() {
    float targetVelocityX = moveSpeed;
    if (timeSinceHitByBlue < blueHitLinger) {
      targetVelocityX = moveSpeed/2;
      timeSinceHitByBlue += Time.deltaTime;
    }
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeGrounded);
    velocity.y = gravity * Time.deltaTime;
  }

  public void HitByRedBullet(RaycastHit2D hit) {
    if (Time.time > timeSinceHitByRed) {
      timeSinceHitByRed = Time.time + 1/redHitPerSecond;
      health -= 1f;
      print(gameObject.name + "HP: " + health + "/" + startingHealth);
      if (health <= 0) {
        Die();
      }
    }
  }

  public void HitByBlueBullet(RaycastHit2D hit) {
    timeSinceHitByBlue = 0;
  }

  public void HitByYellowBullet(RaycastHit2D hit) {
    //calucate direction and apply vector
    velocity.x += -yellowHitForce;
  }

  private void Die() {
    Destroy(gameObject);
  }
}
