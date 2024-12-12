using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Runner : Agent
{
    public Transform runner;
    public Transform boy;
    public Transform fox;
    public float speed = 5.0f;

    private Rigidbody runnerRb;
    private float disB;
    private float disF;

    public override void Initialize()
    {
        // 러너 Rigidbody 초기화
        runnerRb = runner.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // 에피소드 초기화: 러너와 소년의 위치를 랜덤으로 설정
        runner.transform.localPosition = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        boy.transform.localPosition = new Vector3(Random.Range(6, 10), 0, Random.Range(6, 10));
        disB = Vector3.Distance(boy.localPosition, runner.localPosition);
        disF = Vector3.Distance(fox.localPosition, runner.localPosition);

        // 러너의 속도를 초기화
        runnerRb.velocity = Vector3.zero;
        runnerRb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 러너와 소년, 여우 간 거리 관찰 추가
        sensor.AddObservation(disB);
        sensor.AddObservation(disF);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // 거리 업데이트
        disB = Vector3.Distance(boy.localPosition, runner.localPosition);

        Vector3 force = Vector3.zero;

        // 행동 처리
        switch (actionBuffers.DiscreteActions[0])
        {
            case 0: 
                force = Vector3.forward * speed; 
                break; // 전방 이동
            case 1: 
                force = Vector3.back * speed; 
                break; // 후방 이동
            case 2: 
                force = Vector3.left * speed; 
                break; // 좌측 이동
            case 3: 
                force = Vector3.right * speed; 
                break; // 우측 이동
            case 4: 
                force = Vector3.forward * 2 * speed; // Dash
                AddReward(-0.002f); // Dash 시 보상 추가
                break;
        }

        // Rigidbody에 힘 적용
        runnerRb.AddForce(force * Time.deltaTime, ForceMode.VelocityChange);

        // 보상 및 에피소드 종료 조건
        if (disB <= 1.0f)
        {
            AddReward(1.0f); // 목표 도달 보상
            EndEpisode();
        }
        else
        {
            AddReward(-0.001f); // 시간 경과에 따른 작은 페널티
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 수동 제어 입력 처리
        var discreteActions = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W)) discreteActions[0] = 0; // 전방 이동
        else if (Input.GetKey(KeyCode.S)) discreteActions[0] = 1; // 후방 이동
        else if (Input.GetKey(KeyCode.A)) discreteActions[0] = 2; // 좌측 이동
        else if (Input.GetKey(KeyCode.D)) discreteActions[0] = 3; // 우측 이동
        else if (Input.GetKey(KeyCode.LeftShift)) discreteActions[0] = 4; // Dash
        else discreteActions[0] = 0;
    }
}
