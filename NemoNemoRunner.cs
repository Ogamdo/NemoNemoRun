using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NemoNemoRunner : Agent
{
    public Transform target;   // ��ǥ ������Ʈ�� ��ġ
    public Transform obstacle; // ���ع� ������Ʈ�� ��ġ
    public Animator animator;  // �ִϸ����� ������Ʈ

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float sideMoveDistance = 1f; // �¿� �̵� �Ÿ�

    private Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // ���Ǽҵ� ���� �� ȣ��
    public override void OnEpisodeBegin()
    {
        // ������Ʈ ��ġ�� �������� �ʱ�ȭ
        this.transform.localPosition = new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f));

        // ��ǥ ��ġ�� �������� ���� (-10, 10 ���� ��)
        target.localPosition = new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f));

        // ���ع� ��ġ�� �������� ���� (-5, 5 ���� ��)
        obstacle.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        
        // �ӵ� �ʱ�ȭ
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }

    // ���� ����
    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. ��ǥ�� ������Ʈ ���� ��� ��ġ (2���� ��)
        sensor.AddObservation(target.localPosition.x - this.transform.localPosition.x); // x ��� ��ġ
        sensor.AddObservation(target.localPosition.z - this.transform.localPosition.z); // z ��� ��ġ

        // 2. ���ع��� ������Ʈ ���� ��� ��ġ (2���� ��)
        sensor.AddObservation(obstacle.localPosition.x - this.transform.localPosition.x); // x ��� ��ġ
        sensor.AddObservation(obstacle.localPosition.z - this.transform.localPosition.z); // z ��� ��ġ

        // 3. ������Ʈ�� ���� ��ġ (2���� ��)
        sensor.AddObservation(this.transform.localPosition.x); // ���� x ��ġ
        sensor.AddObservation(this.transform.localPosition.z); // ���� z ��ġ

        // 4. ��ǥ���� �浹 ���� (1���� ��)
        sensor.AddObservation(IsCollidingWithTarget() ? 1.0f : 0.0f); // �浹 ���� (0 �Ǵ� 1)
    }

    // �ൿ ���� (�̻���)
    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        // ���� �ִϸ��̼� Ʈ���� �ʱ�ȭ
        animator.SetBool("isMoving", false);

        switch (action)
        {
            case 0: // ����
                rBody.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
                animator.SetBool("isMoving", true); // ���� �ִϸ��̼� Ʈ���� Ȱ��ȭ
                break;
            case 1: // �·� �̵�
                rBody.MovePosition(transform.position + Vector3.left * sideMoveDistance * Time.deltaTime);
                break;
            case 2: // ��� �̵�
                rBody.MovePosition(transform.position + Vector3.right * sideMoveDistance * Time.deltaTime);
                break;
        }

        // �� ���ܸ��� ���� ���Ƽ�� �־� ��ȿ���� �ൿ�� ����
        AddReward(-0.001f);
    }

    // ��ǥ�� �浹�ߴ��� Ȯ��
    private bool IsCollidingWithTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == target)
            {
                return true;
            }
        }
        return false;
    }

    // �浹 ó��
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            SetReward(10.0f); // ��ǥ�� �������� �� ū ����
            EndEpisode();
        }
        else if (collision.transform == obstacle)
        {
            SetReward(-1.0f); // ���ع��� �浹���� �� ���Ƽ
           
            EndEpisode();
        }
    }

    // �޸���ƽ �Լ�: ����� �����ϴ� ������� ������Ʈ ����
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 0; // ����
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 1; // �·� �̵�
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 2; // ��� �̵�
        }
    }
}
