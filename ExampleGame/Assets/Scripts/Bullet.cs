using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Bullet : MonoBehaviour {

  public LayerMask collisionMask;

  [HideInInspector]
  public float speed;
  [HideInInspector]
  public float lifetime;

  private new BoxCollider2D collider;

  private void Start() {
    collider = GetComponent<BoxCollider2D>();
  }

  private void Update() {
    float moveDistance = speed * Time.deltaTime;
    Vector2 translation = Vector2.right * moveDistance;
    CheckCollisions(moveDistance);
    transform.Translate(translation);
    Destroy(gameObject, lifetime);
  }

  private void CheckCollisions(float moveDistance) {
    float rayLength = transform.localScale.x/2 + moveDistance;
    Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
    Debug.DrawRay(rayOrigin, transform.right * rayLength, Color.blue);
    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, collisionMask);

    if (hit) {
      IBulletInteractable hitObject = hit.collider.GetComponent<IBulletInteractable>();
      if (hitObject != null) {
        hitObject.HitByRedBullet(hit);
      }
      Destroy(gameObject);
    }
  }
}
