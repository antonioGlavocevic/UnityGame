using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IBulletInteractable {

  public float startingHealth;

  private float health;
  private float timeBetweenEachRedHit = 1f;
  private float timeSinceHitByRed = 0;

  private void Start() {
    health = startingHealth;
  }

  public void HitByRedBullet(RaycastHit2D hit) {
    if (Time.time > timeSinceHitByRed) {
      timeSinceHitByRed = Time.time + timeBetweenEachRedHit;
      health -= 1f;
      print(gameObject.name + "HP: " + health + "/" + startingHealth);
      if (health <= 0) {
        Die();
      }
    }
  }

  private void Die() {
    Destroy(gameObject);
  }
}
