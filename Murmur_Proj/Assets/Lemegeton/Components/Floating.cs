using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;
    [SerializeField] float offset = 0.5f;
    [SerializeField] Vector3 _orignPos;

    void Start()
    {
        _orignPos = transform.localPosition;
    }
    void Update()
    {
        transform.localPosition = _orignPos + new Vector3(0, offset * Mathf.Cos(Time.time * speed), 0);
    }
}
