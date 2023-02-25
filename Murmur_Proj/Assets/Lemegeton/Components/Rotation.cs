using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float speed = 120f;
    [SerializeField] Transform trans;
    void FixedUpdate()
    {
        trans.Rotate(0f, 0f, -Time.fixedDeltaTime * speed, Space.Self);
    }
}
