using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NemoNemoRunner : Agent
{
    public Transform target;   // 목표 오브젝트의 위치
    public Transform obstacle; // 방해물 오브젝트의 위치
    public Animator animator;  // 애니메이터 컴포넌트

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float sideMoveDistance = 1f; // 좌우 이동 거리

    private Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // 에피소드 시작 시 호출
    public override void OnEpisodeBegin()
    {
        // 에이전트 위치를 랜덤으로 초기화
        this.transform.localPosition = new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f));

        // 목표 위치를 랜덤으로 설정 (-10, 10 범위 내)
        target.localPosition = new Vector3(Random.Range(-10f, 10f), 0.5f, Random.Range(-10f, 10f));

        // 방해물 위치를 랜덤으로 설정 (-5, 5 범위 내)
        obstacle.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        
        // 속도 초기화
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
    }

    // 관찰 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. 목표와 에이전트 간의 상대 위치 (2개의 값)
        sensor.AddObservation(target.localPosition.x - this.transform.localPosition.x); // x 상대 위치
        sensor.AddObservation(target.localPosition.z - this.transform.localPosition.z); // z 상대 위치

        // 2. 방해물과 에이전트 간의 상대 위치 (2개의 값)
        sensor.AddObservation(obstacle.localPosition.x - this.transform.localPosition.x); // x 상대 위치
        sensor.AddObservation(obstacle.localPosition.z - this.transform.localPosition.z); // z 상대 위치

        // 3. 에이전트의 현재 위치 (2개의 값)
        sensor.AddObservation(this.transform.localPosition.x); // 현재 x 위치
        sensor.AddObservation(this.transform.localPosition.z); // 현재 z 위치

        // 4. 목표와의 충돌 여부 (1개의 값)
        sensor.AddObservation(IsCollidingWithTarget() ? 1.0f : 0.0f); // 충돌 여부 (0 또는 1)
    }

    // 행동 설정 (이산적)
    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        // 전진 애니메이션 트리거 초기화
        animator.SetBool("isMoving", false);

        switch (action)
        {
            case 0: // 전진
                rBody.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
                animator.SetBool("isMoving", true); // 전진 애니메이션 트리거 활성화
                break;
            case 1: // 좌로 이동
                rBody.MovePosition(transform.position + Vector3.left * sideMoveDistance * Time.deltaTime);
                break;
            case 2: // 우로 이동
                rBody.MovePosition(transform.position + Vector3.right * sideMoveDistance * Time.deltaTime);
                break;
        }

        // 매 스텝마다 작은 페널티를 주어 비효율적 행동을 줄임
        AddReward(-0.001f);
    }

    // 목표와 충돌했는지 확인
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

    // 충돌 처리
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            SetReward(10.0f); // 목표에 도달했을 때 큰 보상
            EndEpisode();
        }
        else if (collision.transform == obstacle)
        {
            SetReward(-1.0f); // 방해물과 충돌했을 때 페널티
           
            EndEpisode();
        }
    }

    // 휴리스틱 함수: 사람이 조작하는 방식으로 에이전트 제어
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 0; // 전진
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 1; // 좌로 이동
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 2; // 우로 이동
        }
    }
}
