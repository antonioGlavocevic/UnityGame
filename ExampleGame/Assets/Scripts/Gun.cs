using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

  public GameObject bulletPrefab;
  public Transform firePoint;
  public Transform gunPivot;
  public float bulletSpeed = 1f;
  public float bulletLifetime = 2f;
  public float fireRate = 5f;

  float timeToFire = 0f;
  
  private void Update() {
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 difference = mousePosition - gunPivot.position;
    difference.Normalize();
    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
    gunPivot.rotation = Quaternion.Euler (0f, 0f, rotationZ);
  }

  public void Shoot () {
    if (Time.time > timeToFire) {
      Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Bullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
      timeToFire = Time.time + 1/fireRate;
    }
  }
}
