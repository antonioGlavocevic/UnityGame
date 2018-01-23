using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

  public GameObject bullet;
  public Transform firePoint;
  
  private void Update() {
    if (Input.GetKeyDown(KeyCode.RightControl)) {
      Fire();
    }
  }
  public void Fire () {
    Instantiate(bullet, firePoint.position, firePoint.rotation);
  }
}
