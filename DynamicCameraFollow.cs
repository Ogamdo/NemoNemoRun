using UnityEngine;

public class DynamicCameraFollow : MonoBehaviour
{
    public Transform target;         // 따라갈 대상 (에이전트)
    public Vector3 offset = new Vector3(0, 5, -10); // 카메라의 기본 위치 오프셋
    public float followSpeed = 5f;   // 따라가는 속도
    public float rotateSpeed = 100f; // 회전 속도

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. 카메라 위치를 에이전트의 위치에 맞추기 (부드러운 이동)
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1 / followSpeed);

        // 2. 에이전트를 향해 카메라가 회전하도록 설정
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}
