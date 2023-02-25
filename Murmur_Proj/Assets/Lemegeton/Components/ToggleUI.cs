using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] List<Sprite> sprites;
    public int index;

    public void Start()
    {
        //Toggle(index);
    }

    public void Toggle(int index)
    {
        if(index < 0 || index >= sprites.Count) image.sprite = null;
        image.sprite = sprites[index];
    }
}