using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

  public GameObject redBulletPrefab;
  public GameObject blueBulletPrefab;
  public GameObject yellowBulletPrefab;
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

  public void ShootRed () {
    if (Time.time > timeToFire) {
      RedBullet bullet = Instantiate(redBulletPrefab, firePoint.position, firePoint.rotation).GetComponent<RedBullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
      timeToFire = Time.time + 1/fireRate;
    }
  }

  public void ShootBlue () {
    if (Time.time > timeToFire) {
      BlueBullet bullet = Instantiate(blueBulletPrefab, firePoint.position, firePoint.rotation).GetComponent<BlueBullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
      timeToFire = Time.time + 1/fireRate;
    }
  }

  public void ShootYellow() {
    YellowBullet bullet = Instantiate(yellowBulletPrefab, firePoint.position, firePoint.rotation).GetComponent<YellowBullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
  }
}
