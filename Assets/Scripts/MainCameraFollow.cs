using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{
    [SerializeField] private float height = 12f;
    [SerializeField] private float distance = 10f;
    [SerializeField] private float smoothTime = 0.2f;

    private Transform followTarget;
    private Vector3 smoothVelocity;

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
        smoothVelocity = Vector3.zero;

        if (followTarget != null)
        {
            SnapToTarget();
        }
    }

    private Vector3 GetDesiredPosition()
    {
        return followTarget.position + Vector3.up * height + Vector3.back * distance;
    }

    private void SnapToTarget()
    {
        transform.position = GetDesiredPosition();
        transform.rotation = Quaternion.Euler(50f, 0f, 0f);
    }

    private void LateUpdate()
    {
        if (followTarget == null)
        {
            return;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            GetDesiredPosition(),
            ref smoothVelocity,
            smoothTime);

        Vector3 lookTarget = followTarget.position + Vector3.up * 1.5f;
        transform.LookAt(lookTarget);
    }
}
