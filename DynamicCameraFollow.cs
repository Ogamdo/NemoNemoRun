using UnityEngine;

public class DynamicCameraFollow : MonoBehaviour
{
    public Transform target;         // ���� ��� (������Ʈ)
    public Vector3 offset = new Vector3(0, 5, -10); // ī�޶��� �⺻ ��ġ ������
    public float followSpeed = 5f;   // ���󰡴� �ӵ�
    public float rotateSpeed = 100f; // ȸ�� �ӵ�

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. ī�޶� ��ġ�� ������Ʈ�� ��ġ�� ���߱� (�ε巯�� �̵�)
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1 / followSpeed);

        // 2. ������Ʈ�� ���� ī�޶� ȸ���ϵ��� ����
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
}
