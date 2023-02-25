using System;
using System.Collections;
using UnityEngine;
public class StorageHubProgressView : MonoBehaviour
{
    [SerializeField] MeshRenderer _render;
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] GameObject _textObj;
    [SerializeField] TextMesh _textMesh;
    [SerializeField] GameObject _alertObj;

    void Start()
    {
        _render.material.SetFloat("_Board", _meshFilter.mesh.bounds.size.z / 2f);
        _render.material.SetFloat("_Offset", _meshFilter.mesh.bounds.center.z);
    }
    
    public void SetCount(int count, int total, int level)
    {
        if(level <= 0) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
            if(total > 0 && count == total) {
                _textObj.SetActive(false);
                _alertObj.SetActive(true);
            } else {
                _textObj.SetActive(true);
                _textMesh.text = count.ToString();
                _alertObj.SetActive(false);
            }
            var progress = (float) count / total;
            _render.material.SetFloat("_Progress",  progress*1.4f - 0.2f);
        }
    }
}
