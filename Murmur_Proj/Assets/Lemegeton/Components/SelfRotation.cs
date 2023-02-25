using UnityEngine;

public class SelfRotation : MonoBehaviour
{
    [SerializeField] float speed = 120f;
    void FixedUpdate()
    {
        transform.Rotate(0f, 0f, -Time.fixedDeltaTime * speed, Space.Self);
    }
}
