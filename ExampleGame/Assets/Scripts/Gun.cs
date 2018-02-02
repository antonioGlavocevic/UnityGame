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

  public float maxSustainRed = 4f;
  public float secondsOfRedLeft;
  public float rechargeDelay = 1f;
  public float timeUntilRecharge = 0;
  public float rechargeMultiplier = 1f;
  public bool firing = false;
  
  float timeToFire = 0;

  private void Start() {
    secondsOfRedLeft = maxSustainRed;
  }

  private void Update() {
    if (timeUntilRecharge <= 0) {
      secondsOfRedLeft += Time.deltaTime * rechargeMultiplier;
      secondsOfRedLeft = Mathf.Clamp(secondsOfRedLeft,0,maxSustainRed);
    }
    if (firing) {
      ShootRed();
      secondsOfRedLeft -= Time.deltaTime;
      secondsOfRedLeft = Mathf.Clamp(secondsOfRedLeft,0,maxSustainRed);
      timeUntilRecharge = rechargeDelay;
    }
    else {
      timeUntilRecharge -= Time.deltaTime * rechargeMultiplier;
      timeUntilRecharge = Mathf.Clamp(timeUntilRecharge,0,rechargeDelay);
    }
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 difference = mousePosition - gunPivot.position;
    difference.Normalize();
    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
    gunPivot.rotation = Quaternion.Euler (0f, 0f, rotationZ);
  }

  public void ShootRed () {
    if (timeToFire <= 0 && secondsOfRedLeft > 0) {
      Bullet bullet = Instantiate(redBulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Bullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
      timeToFire = 1/fireRate;
    }
  }

  public void ShootBlue () {
    timeToFire -= Time.deltaTime;
    if (timeToFire <= 0) {
      Bullet bullet = Instantiate(blueBulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Bullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
      timeToFire = 1/fireRate;
    }
  }

  public void ShootYellow() {
      Bullet bullet = Instantiate(yellowBulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Bullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
  }
}
