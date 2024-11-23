using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Runner : Agent
{
    // 목표 Transform (Inspector에서 설정)
    public Transform targetTransform;

    // 필드 GameObject (Inspector에서 설정)
    public GameObject field;

    // 에이전트 이동 속도
    public float moveSpeed = 5f;

    // 초기 거리 저장
    private float initialDistance;

    // 에이전트 초기화
    public override void Initialize()
    {
        // 초기 거리 계산
        initialDistance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
    }

    // 에피소드 시작 시 호출
    public override void OnEpisodeBegin()
    {
        // 위치 초기화 제거 (Lover와 에이전트의 기존 위치 유지)
        // 초기 거리 재계산 (학습 진행 상황에 따라 필요)
        initialDistance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
    }

    // 관측 데이터 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        // 목표와 에이전트의 상대 위치 관측
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);

        // 에이전트의 현재 위치 관측
        sensor.AddObservation(transform.localPosition);

        // 필드 경계 크기 관측 (필드가 설정된 경우)
        if (field != null)
        {
            Bounds fieldBounds = field.GetComponent<Collider>().bounds;
            sensor.AddObservation(fieldBounds.size);
        }
    }

    // 행동을 받아 에이전트 이동
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f); // X축 이동
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f); // Z축 이동

        // 에이전트 이동
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        // 필드 영역 벗어남 확인
        if (!IsWithinFieldBounds(transform.localPosition))
        {
            SetReward(-10f); // 필드 이탈 패널티
            EndEpisode();
        }

        // 목표와의 거리 계산
        float currentDistance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);

        // 목표에 가까워지면 보상 제공
        if (currentDistance < initialDistance)
        {
            float distanceReward = (initialDistance - currentDistance) / initialDistance;
            AddReward(distanceReward);
            initialDistance = currentDistance; // 현재 거리를 업데이트
        }
    }

    // 목표 지점 도달 시 보상 및 에피소드 종료
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == targetTransform)
        {
            SetReward(1f); // 목표 도달 시 최대 보상
            EndEpisode();
        }
    }

    // 수동 조작을 위한 Heuristic 설정
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal"); // 수평 입력
        continuousActions[1] = Input.GetAxis("Vertical");   // 수직 입력
    }

    // 필드 경계 내에 있는지 확인
    private bool IsWithinFieldBounds(Vector3 position)
    {
        if (field == null)
        {
            Debug.LogWarning("Field GameObject가 설정되지 않았습니다.");
            return true; // 필드가 설정되지 않았다면 항상 true
        }

        Bounds fieldBounds = field.GetComponent<Collider>().bounds;
        return fieldBounds.Contains(position); // 필드 내부인지 확인
    }
}
