using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
   protected static T _instance;

   public static T Instance
   {
      get
      {
         if (_instance is null)
         {
            _instance = (T)FindObjectOfType(typeof(T));

            if (_instance is null)
            {
               GameObject obj = new GameObject();
               obj.name = typeof(T).Name;
               _instance = obj.AddComponent<T>();
            }
         }
         
         return _instance;
      }
   }

   public virtual void Awake()
   {
      if (!_instance)
      {
         _instance = this as T;
         DontDestroyOnLoad(this);
      }
      else
      {
         Destroy(gameObject);
      }
   }
}
