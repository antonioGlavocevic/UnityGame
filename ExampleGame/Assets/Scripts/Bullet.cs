using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  
  public LayerMask collisionMask;
  public Color bulletColor;
  public BulletStats bulletStats;
  
  [HideInInspector]
  public float speed;
  [HideInInspector]
  public float lifetime;

  void Start() {
    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    meshRenderer.material.color = bulletColor;
  }
  
  void OnEnable() {
    Invoke("Die",lifetime);
  }

  void Update() {
    float moveDistance = speed * Time.deltaTime;
    Vector2 translation = Vector2.right * moveDistance;
    CheckCollisions(moveDistance);
    transform.Translate(translation);
  }

  void CheckCollisions(float moveDistance) {
    float rayLength = transform.localScale.x/2 + moveDistance;
    Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
    Debug.DrawRay(rayOrigin, transform.right * rayLength, Color.blue);
    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, collisionMask);

    if (hit) {
      IBulletInteractable hitObject = hit.collider.GetComponent<IBulletInteractable>();
      if (hitObject != null) {
        OnHitObject(hitObject, hit);
      }
      Die();
    }
  }

  void Die() {
    gameObject.SetActive(false);
  }

  void OnHitObject(IBulletInteractable hitObject, RaycastHit2D hit) {
    hitObject.HitByBullet(hit, bulletStats);
  }

  public enum BulletType {
    Red, Blue, Purple, Yellow, Orange, Green, Black
  }

  [System.Serializable]
  public struct BulletStats {
    public BulletType bulletType;
    public float damage;
    public float slow;
    public float knockbackForce;
  }
}
