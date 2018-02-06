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

  public float bulletLifetime;
  public float bulletTravelDistance;
  float bulletSpeed;
  public float streamFireRate;
  float _timeToStreamFire = 0;
  float timeToStreamFire {
    get { return _timeToStreamFire; }
    set { _timeToStreamFire = Mathf.Clamp(value,0,1/streamFireRate); }
  }
  public float blastFireDelay;
  float _timeToBlastFire = 0;
  float timeToBlastFire {
    get { return _timeToBlastFire; }
    set { _timeToBlastFire = Mathf.Clamp(value,0,blastFireDelay); }
  }

  [HideInInspector]
  public bool redTrigger = false;
  [HideInInspector]
  public bool blueTrigger = false;
  [HideInInspector]
  public bool yellowTrigger = false;

  float deltaTime;

  private void Awake() {
    bulletSpeed = bulletTravelDistance/bulletLifetime;
    Bullet bullet = redBulletPrefab.GetComponent<Bullet>();
    bullet.speed = bulletSpeed;
    bullet.lifetime = bulletLifetime;
    bullet = blueBulletPrefab.GetComponent<Bullet>();
    bullet.speed = bulletSpeed;
    bullet.lifetime = bulletLifetime;
    bullet = yellowBulletPrefab.GetComponent<Bullet>();
    bullet.speed = bulletSpeed;
    bullet.lifetime = bulletLifetime;
    ObjectPooler.Instance.CreatePool(redBulletPrefab, 10, true);
    ObjectPooler.Instance.CreatePool(blueBulletPrefab, 10, true);
    ObjectPooler.Instance.CreatePool(yellowBulletPrefab, 5, true);
  }

  private void Start() {
    redTank.Setup();
    blueTank.Setup();
    yellowTank.Setup();
  }

  private void Update() {
    deltaTime = Time.deltaTime;
    timeToStreamFire -= deltaTime;
    timeToBlastFire -= deltaTime;

    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 difference = mousePosition - gunPivot.position;
    difference.Normalize();
    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
    gunPivot.rotation = Quaternion.Euler (0f, 0f, rotationZ);

    redTank.Cooldown(deltaTime);
    blueTank.Cooldown(deltaTime);
    yellowTank.Cooldown(deltaTime);
    if (redTrigger && !redTank.isOverheated) {
      ShootRed();
    }
    else if (blueTrigger && !blueTank.isOverheated) {
      ShootBlue();
    }
    else if (yellowTrigger && !yellowTank.isOverheated) {
      ShootYellow();
    }
    redTrigger = false;
    blueTrigger = false;
    yellowTrigger = false;
  }

  public void ShootRed () {
    if (timeToStreamFire <= 0) {
      GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(redBulletPrefab);
      if (bulletObj != null) {
        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;
        bulletObj.SetActive(true);

        timeToStreamFire = 1/streamFireRate;
      }
    }
    redTank.StreamFired(deltaTime);
  }

  public void ShootBlue () {
    if (timeToStreamFire <= 0) {
      GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(blueBulletPrefab);
      if (bulletObj != null) {
        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;
        bulletObj.SetActive(true);

        timeToStreamFire = 1/streamFireRate;
      }
    }
    blueTank.StreamFired(deltaTime);
  }

  public void ShootYellow() {
    if (timeToBlastFire <= 0) {
      for (int i = -2; i <= 2; i++)
      {
        GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(yellowBulletPrefab);
        if (bulletObj != null) {
          bulletObj.transform.position = firePoint.position;
          Vector3 rotation = new Vector3(firePoint.eulerAngles.x,firePoint.eulerAngles.y,firePoint.eulerAngles.z + (i * 5.625f));
          bulletObj.transform.rotation = Quaternion.Euler(rotation);
          bulletObj.SetActive(true);
        }
        timeToBlastFire = blastFireDelay;
      }
      yellowTank.BlastFired();
    }
  }

  [System.Serializable]
  public struct GasTank {
    public float timeToOverheat;
    [Range (0,100)]
    public float blastHeatPercent;
    float maxHeat;
    float heatPerSecond;
    float blastHeat;

    float _currentHeat;
    public float currentHeat {
      get { return _currentHeat; }
      set { _currentHeat = Mathf.Clamp(value, 0, maxHeat); }
    }

    public float cooldownDelay;
    public float cooldownTime;
    float cooldownPerSecond;

    float _cooldownDelayRemaining;
    public float cooldownDelayRemaining {
      get { return _cooldownDelayRemaining; }
      set { _cooldownDelayRemaining = Mathf.Clamp(value, 0, cooldownDelay); }
    }

    bool overheated;
    public bool isOverheated {
      get { return overheated; }
    }

    public void Setup() {
      maxHeat = 100;
      currentHeat = 0;
      heatPerSecond = maxHeat * 1/timeToOverheat;
      blastHeat = maxHeat * blastHeatPercent/100;
      cooldownDelayRemaining = 0;
      cooldownPerSecond = maxHeat * 1/cooldownTime;
      overheated = false;
    }

    public void Cooldown(float deltaTime) {
      if (currentHeat == 0) {
        overheated = false;
      }
      if (cooldownDelayRemaining == 0) {
        currentHeat -= cooldownPerSecond * deltaTime;
      }
      cooldownDelayRemaining -= deltaTime;
    }

    public void StreamFired(float deltaTime) {
      IncreaseHeat(heatPerSecond * deltaTime);
    }

    public void BlastFired() {
      IncreaseHeat(blastHeat);
    }

    private void IncreaseHeat(float heat) {
      cooldownDelayRemaining = cooldownDelay;
      currentHeat += heat;
      if (currentHeat == maxHeat) {
        overheated = true;
      }
    }
  }
}
