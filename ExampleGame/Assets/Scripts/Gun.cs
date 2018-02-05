using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

  public GameObject redBulletPrefab;
  public GasTank redTank;
  public GameObject blueBulletPrefab;
  public GasTank blueTank;
  public GameObject yellowBulletPrefab;
  public GasTank yellowTank;
  public Transform firePoint;
  public Transform gunPivot;

  public float bulletSpeed = 1f;
  public float bulletLifetime = 2f;
  public float streamFireRate = 5f;
  float timeToStreamFire = 0;
  public float blastFireDelay = 2f;
  float timeToBlastFire = 0;

  public bool redTrigger = false;
  public bool blueTrigger = false;
  public bool yellowTrigger = false;

  [System.Serializable]
  public struct GasTank {
    public float fireTime;
    public float fireTimeRemaining;
    /*float fireTimeRemaining {
      get { return fireTimeRemaining; }
      set { fireTimeRemaining = Mathf.Clamp(value,0,fireTime); }
    }*/
    public float rechargeDelay;
    public float rechargeDelayRemaining;
    /*float rechargeDelayRemaining {
      get { return rechargeDelayRemaining; }
      set { rechargeDelayRemaining = Mathf.Clamp(value,0,rechargeDelay); }
    }*/
    public float rechargeMultiplier;
    bool lockout;

    public void Setup() {
      fireTimeRemaining = fireTime;
      rechargeDelayRemaining = rechargeDelay;
      lockout = false;
    }

    public void Update(float deltaTime) {
      if (fireTimeRemaining == fireTime) {
        lockout = false;
      }
      if (rechargeDelayRemaining <= 0) {
        fireTimeRemaining += deltaTime * rechargeMultiplier;
        fireTimeRemaining = Mathf.Clamp(fireTimeRemaining,0,fireTime);
      }
      rechargeDelayRemaining -= deltaTime;
      rechargeDelayRemaining = Mathf.Clamp(rechargeDelayRemaining,0,rechargeDelay);
    }

    public bool CanStreamFire(float deltaTime) {
      if (!lockout && fireTimeRemaining > 0) {
        rechargeDelayRemaining = rechargeDelay;
        fireTimeRemaining -= deltaTime;
        fireTimeRemaining = Mathf.Clamp(fireTimeRemaining,0,fireTime);
        return true;
      }
      if (fireTimeRemaining <= 0) {
        lockout = true;
      }
      return false;
    }

    public bool CanBlastFire() {
      if (!lockout && fireTimeRemaining > (fireTime * 0.25f * 0.25f)) {
        rechargeDelayRemaining = rechargeDelay;
        fireTimeRemaining -= fireTime * 0.25f;
        fireTimeRemaining = Mathf.Clamp(fireTimeRemaining,0,fireTime);
        return true;
      }
      if (fireTimeRemaining <= 0) {
        lockout = true;
      }
      return false;
    }
  }

  private void Awake() {
    ObjectPooler.Instance.CreatePool(redBulletPrefab, 3, true);
    ObjectPooler.Instance.CreatePool(blueBulletPrefab, 3, true);
    ObjectPooler.Instance.CreatePool(yellowBulletPrefab, 3, true);
  }

  private void Start() {
    redTank.Setup();
    blueTank.Setup();
    yellowTank.Setup();
  }

  private void Update() {
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 difference = mousePosition - gunPivot.position;
    difference.Normalize();
    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
    gunPivot.rotation = Quaternion.Euler (0f, 0f, rotationZ);

    timeToStreamFire -= Time.deltaTime;
    timeToStreamFire = Mathf.Clamp(timeToStreamFire,0,streamFireRate);
    timeToBlastFire -= Time.deltaTime;
    timeToBlastFire = Mathf.Clamp(timeToBlastFire,0,blastFireDelay);

    redTank.Update(Time.deltaTime);
    blueTank.Update(Time.deltaTime);
    yellowTank.Update(Time.deltaTime);
    if (redTrigger) {
      ShootRed();
    }
    else if (blueTrigger) {
      ShootBlue();
    }
    else if (yellowTrigger) {
      ShootYellow();
    }
    redTrigger = false;
    blueTrigger = false;
    yellowTrigger = false;
  }

  public void ShootRed () {
    if (redTank.CanStreamFire(Time.deltaTime) && timeToStreamFire <= 0) {
      GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(redBulletPrefab);
      if (bulletObj != null) {
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
        bullet.lifetime = bulletLifetime;

        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;
        bulletObj.SetActive(true);

        timeToStreamFire = 1/streamFireRate;
      }
    }
  }

  public void ShootBlue () {
    if (blueTank.CanStreamFire(Time.deltaTime) && timeToStreamFire <= 0) {
      GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(blueBulletPrefab);
      if (bulletObj != null) {
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
        bullet.lifetime = bulletLifetime;

        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;
        bulletObj.SetActive(true);

        timeToStreamFire = 1/streamFireRate;
      }
    }
  }

  public void ShootYellow() {
    if (timeToBlastFire <= 0 && yellowTank.CanBlastFire()) {
      GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(yellowBulletPrefab);
      if (bulletObj != null) {
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
        bullet.lifetime = bulletLifetime;

        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;
        bulletObj.SetActive(true);

        timeToBlastFire = blastFireDelay;
      }
    }
  }
}
