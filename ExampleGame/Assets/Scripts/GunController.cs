using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

  public Gun gun;
  
  public void ShootRed() {
    gun.ShootRed();
  }

  public void ShootBlue() {
    gun.ShootBlue();
  }

  public void ShootYellow() {
    gun.ShootYellow();
  }
}
