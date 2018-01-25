using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBullet : Bullet {

  protected override void Start() {
    base.Start();
    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    meshRenderer.material.color = new Color(0,153,255);
  }

  protected override void OnHitObject(IBulletInteractable hitObject, RaycastHit2D hit) {
    hitObject.HitByBlueBullet(hit);
  }
}