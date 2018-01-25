using UnityEngine;

public interface IBulletInteractable {
  void HitByRedBullet(RaycastHit2D hit);
  void HitByBlueBullet(RaycastHit2D hit);
  void HitByYellowBullet(RaycastHit2D hit);
}