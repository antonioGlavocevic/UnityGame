﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

  public GasLevelIndicator gasLevelIndicator;

  public List<GameObject> bulletPrefabs;
  public GasTank redTank;
  public GasTank blueTank;
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

  void Awake() {
    bulletSpeed = bulletTravelDistance/bulletLifetime;
    foreach (GameObject bulletPrefab in bulletPrefabs) {
      Bullet bullet = bulletPrefab.GetComponent<Bullet>();
      bullet.speed = bulletSpeed;
      bullet.lifetime = bulletLifetime;
      ObjectPooler.Instance.CreatePool(bulletPrefab, 10, true);
    }
  }

  void Start() {
    redTank.Setup();
    blueTank.Setup();
    yellowTank.Setup();
  }

  void Update() {
    RotateGun();
    ManageCooldowns();
    if (gasLevelIndicator != null) {
      gasLevelIndicator.SetLevels(redTank.currentHeat/redTank.maxHeat, blueTank.currentHeat/blueTank.maxHeat, yellowTank.currentHeat/yellowTank.maxHeat);
    }
  }

  void RotateGun() {
    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 difference = mousePosition - gunPivot.position;
    difference.Normalize();
    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
    gunPivot.rotation = Quaternion.Euler (0f, 0f, rotationZ);
  }

  void ManageCooldowns() {
    deltaTime = Time.deltaTime;
    timeToStreamFire -= deltaTime;
    timeToBlastFire -= deltaTime;

    redTank.Cooldown(deltaTime);
    blueTank.Cooldown(deltaTime);
    yellowTank.Cooldown(deltaTime);
  }

  public void Shoot() {
    bool streamFire = false;
    bool blastFire = false;
    byte activeMask = 0;
    List<GasTank> tanks = new List<GasTank>();
    if (redTrigger && !redTank.isOverheated) {
      tanks.Add(redTank);
      streamFire = true;
      activeMask |= 1;
    }
    if (blueTrigger && !blueTank.isOverheated) {
      tanks.Add(blueTank);
      streamFire = true;
      activeMask |= 2;
    }
    if (yellowTrigger && !yellowTank.isOverheated) {
      tanks.Add(yellowTank);
      blastFire = true;
      activeMask |= 4;
    }

    GameObject bulletPrefab = (activeMask > 0 && activeMask <= bulletPrefabs.Count) ? bulletPrefabs[activeMask-1] : null;
    if (bulletPrefab != null) {
      if (blastFire) {
        bool superBlast = activeMask == 7;
        BlastFire(bulletPrefab, tanks, superBlast);
      }
      else if (streamFire) {
        StreamFire(bulletPrefab, tanks);
      }
    }
  }

  GameObject SelectBullet(byte activeMask) {
    int index = activeMask - 1;
    if (index >= 0 && index < bulletPrefabs.Count) {
      return bulletPrefabs[activeMask-1];
    }
    return null;
  }

  void StreamFire(GameObject bulletPrefab, List<GasTank> tanks) {
    if (timeToStreamFire == 0) {
      GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(bulletPrefab);
      if (bulletObj != null) {
        bulletObj.transform.position = firePoint.position;
        bulletObj.transform.rotation = firePoint.rotation;
        bulletObj.SetActive(true);

        timeToStreamFire = 1/streamFireRate;
      }
    }
    foreach (GasTank tank in tanks) {
      tank.StreamFired(deltaTime);
    }
  }

  void BlastFire(GameObject bulletPrefab, List<GasTank> tanks, bool superBlast) {
    if (timeToBlastFire == 0) {
      for (int i = -2; i <= 2; i++)
      {
        GameObject bulletObj = ObjectPooler.Instance.GetPooledObject(bulletPrefab);
        if (bulletObj != null) {
          bulletObj.transform.position = firePoint.position;
          Vector3 rotation = new Vector3(firePoint.eulerAngles.x,firePoint.eulerAngles.y,firePoint.eulerAngles.z + (i * 5.625f));
          bulletObj.transform.rotation = Quaternion.Euler(rotation);
          bulletObj.SetActive(true);
        }
        timeToBlastFire = blastFireDelay;
      }
      foreach (GasTank tank in tanks) {
        if (superBlast) {
          tank.SuperBlastFired();
        }
        else {
          tank.BlastFired();
        }
      }
    }
  }

  [System.Serializable]
  public class GasTank {
    public float timeToOverheat;
    [Range (0,100)]
    public float blastHeatPercent;
    [HideInInspector]
    public float maxHeat;
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

    public void SuperBlastFired() {
      IncreaseHeat(maxHeat);
    }

    void IncreaseHeat(float heat) {
      cooldownDelayRemaining = cooldownDelay;
      currentHeat += heat;
      if (currentHeat == maxHeat) {
        overheated = true;
      }
    }
  }
}
