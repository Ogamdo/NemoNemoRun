using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Runner : Agent
{
    private Vector3 targetInitialPosition; // 목표의 초기 위치
    public Transform targetTransform;     // 목표 Transform
    public GameObject field;              // 필드 GameObject
    public float moveSpeed = 50f;         // 에이전트 이동 속도

    private float initialDistance;        // 초기 목표 거리 저장
    private Rigidbody rb;                 // Rigidbody 캐싱
    private Vector3 agentInitialPosition; // 에이전트의 초기 위치

    // 에이전트 초기화
    public override void Initialize()
    {
        // Rigidbody 캐싱
        rb = GetComponent<Rigidbody>();

        // 목표의 초기 위치 저장
        targetInitialPosition = targetTransform.localPosition;
    }

    // 에피소드 시작 시 호출
    public override void OnEpisodeBegin()
    {
        // 에이전트의 초기 위치 저장 (에피소드마다 갱신)
        agentInitialPosition = transform.localPosition;

        // 에이전트와 목표 간 초기 거리 계산
        initialDistance = Vector3.Distance(agentInitialPosition, targetInitialPosition);
    }

    // 관측 데이터 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        // 목표의 초기 위치 관측
        sensor.AddObservation(targetInitialPosition);

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

    // Rigidbody를 통한 행동을 받아 에이전트 이동
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f); // X축 이동
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f); // Z축 이동

        // Rigidbody를 통한 이동
        rb.MovePosition(transform.position + new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed);

        // 필드 경계 확인
        if (!IsWithinFieldBounds(transform.localPosition))
        {
            SetReward(-10f); // 필드 이탈 페널티
            EndEpisode();
        }

        // 목표와의 거리 보상 계산
        float currentDistance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        if (currentDistance < initialDistance)
        {
            AddReward((initialDistance - currentDistance) / initialDistance);
            initialDistance = currentDistance; // 거리 갱신
        }
    }

    // 물리적 충돌 처리
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == targetTransform.gameObject)
        {
            SetReward(10.0f); // 목표 도달 보상
            EndEpisode();
        }
        else
        {
            SetReward(-0.5f); // 목표가 아닌 다른 물체와 충돌 시 페널티 (선택 사항)
        }
    }

    // 필드 경계 확인
    private bool IsWithinFieldBounds(Vector3 position)
    {
        if (field == null)
        {
            Debug.LogWarning("Field GameObject가 설정되지 않았습니다.");
            return true;
        }

        Bounds fieldBounds = field.GetComponent<Collider>().bounds;
        return fieldBounds.Contains(position); // 필드 내부인지 확인
    }
}
