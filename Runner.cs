using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Runner : Agent
{
    public Transform targetTransform; // 목표 위치를 나타내는 Transform 변수
    private float rewardThreshold = 10; // 초기 보상 값

    // 에이전트 초기화
    public override void Initialize()
    {
        // 보상 초기화
        rewardThreshold =10.0f;
    }

    // 에피소드 시작 시 호출되는 함수
    public override void OnEpisodeBegin()
    {
        // 에이전트 위치 초기화: x와 z는 -5~5 사이의 랜덤 값, y는 고정된 값 (예: 0.5)
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        
        // 목표 위치 고정 (필요에 따라 설정)
        targetTransform.localPosition = new Vector3(0, 0.5f, 0);
        
        // 보상 초기화
        rewardThreshold = 10f;
    }

    // 관측 데이터 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        // 목표와 에이전트 사이의 상대 위치를 관측
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);
    }

    // 행동을 수신하는 함수
    public override void OnActionReceived(ActionBuffers actions)
    {
        // 행동 설정 (예: 연속적인 x, z 방향 움직임)
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // 에이전트 이동 (Transform 직접 이동)
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * 5f;

        // 매 프레임마다 보상 감소
        rewardThreshold -= 0.001f;

        // 보상이 0 이하가 되면 에피소드 종료
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
            SetReward(rewardThreshold); // 남아 있는 보상 만큼 지급
            EndEpisode(); // 에피소드 종료
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
