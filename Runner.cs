using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Runner : Agent
{
    public Transform targetTransform; // 목표 위치를 나타내는 Transform 변수
    private float rewardThreshold = 10f; // 초기 보상 값
    private int currentStep; // 현재 스텝 수

    // 에이전트 초기화
    public override void Initialize()
    {
        rewardThreshold = 10f;
        currentStep = 0;
    }

    // 에피소드 시작 시 호출되는 함수
    public override void OnEpisodeBegin()
    {
        // 에이전트 위치 초기화
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        targetTransform.localPosition = new Vector3(0, 0.5f, 0);

        rewardThreshold = 10f; // 보상 초기화
        currentStep = 0;       // 스텝 수 초기화
    }

    // 관측 데이터 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);
    }

    // 행동을 수신하는 함수
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // 에이전트 이동
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * 5f;

        // 스텝 수 증가 및 콘솔 출력
        currentStep++;
        Debug.Log("Current Step: " + currentStep);

        // 보상 감소
        rewardThreshold -= 0.001f;
        if (rewardThreshold <= 0)
        {
            EndEpisode();
        }
    }

    // 목표 충돌 시 보상 부여
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            SetReward(rewardThreshold);
            EndEpisode();
        }
    }

    // 수동 제어를 위한 함수 (디버깅용)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
}
