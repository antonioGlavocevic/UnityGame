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

  public float bulletSpeed = 20f;
  public float bulletLifetime = 0.25f;
  public float streamFireRate = 30f;
  float timeToStreamFire = 0;
  public float blastFireDelay = 2f;
  float timeToBlastFire = 0;

  [HideInInspector]
  public bool redTrigger = false;
  [HideInInspector]
  public bool blueTrigger = false;
  [HideInInspector]
  public bool yellowTrigger = false;

  Dictionary<GameObject,Bullet> bulletDictionary;

  [System.Serializable]
  public struct GasTank {
    public float fireTime;
    float fireTimeRemaining;
    public float rechargeDelay;
    float rechargeDelayRemaining;
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
    bulletDictionary = new Dictionary<GameObject, Bullet>();
    ObjectPooler.Instance.CreatePool(redBulletPrefab, 7, true);
    ObjectPooler.Instance.CreatePool(blueBulletPrefab, 7, true);
    ObjectPooler.Instance.CreatePool(yellowBulletPrefab, 5, true);
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
        if (!bulletDictionary.ContainsKey(bulletObj)) {
          bulletDictionary.Add(bulletObj, bulletObj.GetComponent<Bullet>());
        }
        Bullet bullet = bulletDictionary[bulletObj];
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
        if (!bulletDictionary.ContainsKey(bulletObj)) {
          bulletDictionary.Add(bulletObj, bulletObj.GetComponent<Bullet>());
        }
        Bullet bullet = bulletDictionary[bulletObj];
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
      for (int i = -2; i <= 2; i++)
      {
        GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(yellowBulletPrefab);
        if (bulletObj != null) {
          if (!bulletDictionary.ContainsKey(bulletObj)) {
            bulletDictionary.Add(bulletObj, bulletObj.GetComponent<Bullet>());
          }
          Bullet bullet = bulletDictionary[bulletObj];
          bullet.speed = bulletSpeed;
          bullet.lifetime = bulletLifetime;

          bulletObj.transform.position = firePoint.position;
          Vector3 rotation = new Vector3(firePoint.eulerAngles.x,firePoint.eulerAngles.y,firePoint.eulerAngles.z + (i * 5.625f));
          bulletObj.transform.rotation = Quaternion.Euler(rotation);
          bulletObj.SetActive(true);
        }
        timeToBlastFire = blastFireDelay;
      }
    }
  }
}
