using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Enemy : MonoBehaviour, IBulletInteractable {

  public float startingHealth;
  public float moveSpeed;

  Controller2D controller;

  float health;
  float redHitPerSecond = 4f;
  float timeSinceHitByRed = 0;
  float blueHitLinger = 2f;
  float timeSinceHitByBlue;
  float yellowHitForce = 20f;
  Vector2 velocity;
  float velocityXSmoothing;
  public float accelerationTimeGrounded = .1f;
  float gravity = -50;

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

  public void HitByBullet(RaycastHit2D hit, Bullet.BulletStats bulletStats) {
    switch (bulletStats.bulletType) {
      case Bullet.BulletType.Red: HitByRedBullet(); break;
      case Bullet.BulletType.Blue: HitByBlueBullet(); break;
      case Bullet.BulletType.Yellow: HitByYellowBullet(); break;
    }
  }

  private void HitByRedBullet() {
    if (Time.time > timeSinceHitByRed) {
      timeSinceHitByRed = Time.time + 1/redHitPerSecond;
      health -= 1f;
      if (health <= 0) {
        Die();
      }
    }
  }

  private void HitByBlueBullet() {
    timeSinceHitByBlue = 0;
  }

  private void HitByYellowBullet() {
    //calucate direction and apply vector
    velocity.x += -yellowHitForce;
  }

  private void Die() {
    Destroy(gameObject);
  }
}
