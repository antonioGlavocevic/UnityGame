using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

  [HideInInspector]
  public float speed;
  [HideInInspector]
  public float lifetime;

  private void Update () {
    Vector3 translation = Vector3.right * speed * Time.deltaTime;
    transform.Translate(translation);
    Destroy(gameObject, lifetime);
  }
}
