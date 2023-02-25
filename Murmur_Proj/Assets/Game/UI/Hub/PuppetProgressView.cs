using UnityEngine;
using UnityEngine.UI;

public class PuppetProgressView : MonoBehaviour
{    
    [SerializeField] Image _progressBar;
    [SerializeField] Canvas _canvas;
    private Quaternion _rotation = Quaternion.Euler(40f, 130f, 0);
    public void SetCamera(Camera camera)
    {
        _canvas.worldCamera = camera;
    }
    public void Show(float fill = 0)
    {
        _progressBar.fillAmount = fill;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void SetProgress(float fill)
    {
        _progressBar.fillAmount = fill;
    }
    void Update()
    {
        _canvas.transform.rotation = _rotation;
    }
}
