using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

  public Gun gun;
  
  public void Shoot() {
    gun.Shoot();
  }
}
