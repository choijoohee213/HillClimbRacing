using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
    [SerializeField]
    GameObject[] objectPrefabs;

    List<GameObject> pooledObjs = new List<GameObject>();


    private GameObject Generate(string type, bool isActive) {
        for(int i = 0; i < objectPrefabs.Length; i++) {
            if(objectPrefabs[i].name.Equals(type)) {
                GameObject newObject = Instantiate(objectPrefabs[i]);
                newObject.SetActive(isActive);
                pooledObjs.Add(newObject);
                newObject.name = type;
                return newObject;
            }
        }
        return null;
    }


    public GameObject GetObject(string type) {
        foreach(GameObject obj in pooledObjs) {
            if(obj.name.Equals(type) && !obj.activeInHierarchy) {
                obj.SetActive(true);
                return obj;
            }
        }

        return Generate(type, true);
    }

}