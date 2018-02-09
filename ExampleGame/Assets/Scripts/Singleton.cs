using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
  
  private static T _Instance;
  public static T Instance {
    get {
      if (_Instance == null) {
        _Instance = FindObjectOfType<T>();
      }
      return _Instance;
    }
  }
}