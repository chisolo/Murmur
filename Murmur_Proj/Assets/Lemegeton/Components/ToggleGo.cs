using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGo : MonoBehaviour
{
    [SerializeField] List<GameObject> gameObjects;

    public int index;

    public void Start()
    {
        //Toggle(index);
    }

    public void Toggle(int index)
    {
        for(int i = 0; i < gameObjects.Count; ++i){
            if(i == index) gameObjects[i].SetActive(true);
            else gameObjects[i].SetActive(false);
        }
    }
}
