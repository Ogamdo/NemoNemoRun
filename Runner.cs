using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NemoRunner : Agent
{
    public Transform nemo;           // 네모(Nemo) 에이전트
    public Transform goal;           // 목표 지점
    public float speed = 5f;         // 이동 속도

    public override void Initialize()
    {
        // 초기화 로직
    }

    public override void OnEpisodeBegin()
    {
        // 목표 위치를 X, Z 범위 -10에서 10 사이로 랜덤화
        goal.localPosition = new Vector3(
            Random.Range(-10f, 10f), // X축 범위
            goal.localPosition.y,   // Y축 유지
            Random.Range(-10f, 10f) // Z축 범위
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 네모의 현재 위치를 관찰
        sensor.AddObservation(nemo.localPosition);
        // 목표의 현재 위치를 관찰
        sensor.AddObservation(goal.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // 행동 값 받기 (0: 움직이지 않음, 1: 전진, 2: 후진, 3: 왼쪽, 4: 오른쪽)
        int action = actionBuffers.DiscreteActions[0];

        Vector3 move = Vector3.zero;
        switch (action)
        {
            case 1: // 전진
                move = Vector3.forward * speed * Time.deltaTime;
                break;
            case 2: // 후진
                move = Vector3.back * speed * Time.deltaTime;
                break;
            case 3: // 왼쪽
                move = Vector3.left * speed * Time.deltaTime;
                break;
            case 4: // 오른쪽
                move = Vector3.right * speed * Time.deltaTime;
                break;
        }

        // 이동 적용
        nemo.localPosition += move;

        // 목표 도달 여부 확인
        if (Vector3.Distance(nemo.localPosition, goal.localPosition) < 1f)
        {
            SetReward(1f); // 목표 도달 시 보상
            EndEpisode(); // 에피소드 종료
        }

        // 가만히 있는 경우 손실 부여
        if (action == 0) // 행동이 "가만히 있음"일 경우
        {
            AddReward(-0.001f); // 손실 추가
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 수동 제어를 위한 로직
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0; // 기본값: 움직이지 않음

        if (Input.GetKey(KeyCode.W)) discreteActions[0] = 1; // 전진
        if (Input.GetKey(KeyCode.S)) discreteActions[0] = 2; // 후진
        if (Input.GetKey(KeyCode.A)) discreteActions[0] = 3; // 왼쪽
        if (Input.GetKey(KeyCode.D)) discreteActions[0] = 4; // 오른쪽
    }
}
