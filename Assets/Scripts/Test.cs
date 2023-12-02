//using Sirenix.OdinInspector;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.InputSystem;

//[Serializable]
//public class TestFather
//{

//}

//[Serializable]
//public class TestChild : TestFather
//{
//    public int id = 10;
//    public string name = "测试";
//    public int age = 100086;
//}

//public class Test : MonoBehaviour
//{
//    [SerializeField] TestFather testFather = new TestChild();

//    [SerializeField] GameObject effect;
//    GameObject effctGameObj;

//    [SerializeField, Range(0f, 1f)] float time;

//    [DrawDictionary(typeof(int), typeof(GameObject))]
//    public Dictionary<int, GameObject> effects = new Dictionary<int, GameObject>(2) { };

//    private void Start()
//    {
//        //effects.Keys
//    }

//    [Button]
//    public void Init()
//    {
//        effects.Add(10, gameObject);
//    }

//    [Button]
//    public void GetCube()
//    {
//        Addressables.InstantiateAsync("Cube");
//    }

//    private void OnValidate()
//    {
//        if (effect == null) return;

//        if (effctGameObj == null)
//        {
//            effctGameObj = GameObject.Instantiate(effect);
//        };

//        ParticleSystem[] particleSystems = effctGameObj.GetComponentsInChildren<ParticleSystem>();
//        for (int i = 0; i < particleSystems.Length; i++)
//        {
//            particleSystems[i].Simulate(time);
//        }
//    }
//}
