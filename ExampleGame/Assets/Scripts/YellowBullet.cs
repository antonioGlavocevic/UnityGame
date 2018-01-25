using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowBullet : Bullet {

  protected override void Start() {
    base.Start();
    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    meshRenderer.material.color = new Color(255,255,0);
  }

  protected override void OnHitObject(IBulletInteractable hitObject, RaycastHit2D hit) {
    hitObject.HitByYellowBullet(hit);
  }
}