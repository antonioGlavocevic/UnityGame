using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
  
  static T _Instance;
  public static T Instance {
    get {
      if (_Instance == null) {
        _Instance = FindObjectOfType<T>();
      }
      return _Instance;
    }
  }
}