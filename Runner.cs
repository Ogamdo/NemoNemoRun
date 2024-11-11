using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Runner : Agent
{
    public Transform targetTransform; // ��ǥ ��ġ�� ��Ÿ���� Transform ����
    private float rewardThreshold = 10f; // �ʱ� ���� ��
    private int currentStep; // ���� ���� ��

    // ������Ʈ �ʱ�ȭ
    public override void Initialize()
    {
        rewardThreshold = 10f;
        currentStep = 0;
    }

    // ���Ǽҵ� ���� �� ȣ��Ǵ� �Լ�
    public override void OnEpisodeBegin()
    {
        // ������Ʈ ��ġ �ʱ�ȭ
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        targetTransform.localPosition = new Vector3(0, 0.5f, 0);

        rewardThreshold = 10f; // ���� �ʱ�ȭ
        currentStep = 0;       // ���� �� �ʱ�ȭ
    }

    // ���� ������ ����
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);
    }

    // �ൿ�� �����ϴ� �Լ�
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // ������Ʈ �̵�
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * 5f;

        // ���� �� ���� �� �ܼ� ���
        currentStep++;
        Debug.Log("Current Step: " + currentStep);

        // ���� ����
        rewardThreshold -= 0.001f;
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
            SetReward(rewardThreshold);
            EndEpisode();
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
