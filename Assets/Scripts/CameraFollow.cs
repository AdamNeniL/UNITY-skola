using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothTime = 0.15f;
    [SerializeField] Vector3 offset;

    Vector3 velocity;

    void FixedUpdate()
    {
        if (!target) return;

        Vector3 smoothed = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);

        float pixelSize = 1f / 200f;
        smoothed.x = Mathf.Round(smoothed.x / pixelSize) * pixelSize;
        smoothed.y = Mathf.Round(smoothed.y / pixelSize) * pixelSize;

        transform.position = smoothed;
    }
}
