using UnityEngine;

public interface IBulletInteractable {
  void HitByBullet(RaycastHit2D hit, Bullet.BulletStats bullet);
}