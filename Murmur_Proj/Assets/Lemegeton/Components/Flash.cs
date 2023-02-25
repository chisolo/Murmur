using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasGroup))]
public class Flash : MonoBehaviour
{
    [SerializeField] float _durtion = 1f;

    private CanvasGroup _canvasGroup;
    private float time;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        time = 0;
        _canvasGroup.alpha = 1;
    }


    void Update()
    {
        time += Time.deltaTime;

        if (time > 2 * _durtion) {
            time = 0;
        }

        var alpha = Mathf.Abs(time - _durtion) / _durtion;

        _canvasGroup.alpha = alpha;
    }
}
