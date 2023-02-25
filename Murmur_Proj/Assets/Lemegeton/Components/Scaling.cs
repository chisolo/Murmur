using UnityEngine;

public class Scaling : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;
    [SerializeField] float offset = 0.5f;
    [SerializeField] Vector3 _orignScale;

    void Start()
    {
        _orignScale = transform.localScale;
    }
    void Update()
    {
        transform.localScale = _orignScale + new Vector3(offset * Mathf.Cos(Time.time * speed), offset * Mathf.Cos(Time.time * speed), 0);
    }
}
