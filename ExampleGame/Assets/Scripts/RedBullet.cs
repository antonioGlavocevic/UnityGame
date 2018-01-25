using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBullet : Bullet {

  protected override void Start() {
    base.Start();
    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    meshRenderer.material.color = new Color(255,0,0);
  }

  protected override void OnHitObject(IBulletInteractable hitObject, RaycastHit2D hit) {
    hitObject.HitByRedBullet(hit);
  }
}