using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Runner : Agent
{
    public Transform targetTransform; // ��ǥ ��ġ�� ��Ÿ���� Transform ����
    private float rewardThreshold = 10; // �ʱ� ���� ��

    // ������Ʈ �ʱ�ȭ
    public override void Initialize()
    {
        // ���� �ʱ�ȭ
        rewardThreshold =10.0f;
    }

    // ���Ǽҵ� ���� �� ȣ��Ǵ� �Լ�
    public override void OnEpisodeBegin()
    {
        // ������Ʈ ��ġ �ʱ�ȭ: x�� z�� -5~5 ������ ���� ��, y�� ������ �� (��: 0.5)
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        
        // ��ǥ ��ġ ���� (�ʿ信 ���� ����)
        targetTransform.localPosition = new Vector3(0, 0.5f, 0);
        
        // ���� �ʱ�ȭ
        rewardThreshold = 10f;
    }

    // ���� ������ ����
    public override void CollectObservations(VectorSensor sensor)
    {
        // ��ǥ�� ������Ʈ ������ ��� ��ġ�� ����
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);
    }

    // �ൿ�� �����ϴ� �Լ�
    public override void OnActionReceived(ActionBuffers actions)
    {
        // �ൿ ���� (��: �������� x, z ���� ������)
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // ������Ʈ �̵� (Transform ���� �̵�)
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * 5f;

        // �� �����Ӹ��� ���� ����
        rewardThreshold -= 0.001f;

        // ������ 0 ���ϰ� �Ǹ� ���Ǽҵ� ����
        if (rewardThreshold <= 0)
        {
            EndEpisode();
        }
    }

    // ��ǥ �浹 �� ���� �ο�
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            SetReward(rewardThreshold); // ���� �ִ� ���� ��ŭ ����
            EndEpisode(); // ���Ǽҵ� ����
        }
    }

    // ���� ��� ���� �Լ� (������)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
