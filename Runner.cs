using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Runner : Agent
{
    public Transform runner;           // 네모(runner) 에이전트
    public Transform lover;           // 도달해야할 연인
    public float speed = 10f; // 이동 속도  
    /*
     현재는 lover의 위치가 고정 되어 있다. 만약에 lover가 더 빠른 속도로 움직이고, 에이전트가 현재의 보상을 깍으면 speed +=100도 방법일듯? */
          
    
    [HideInInspector] public float dis= 0;
 
    public override void Initialize()
    {
        // 초기화 로직
    }

    public override void OnEpisodeBegin()
    {
       
        // 목표 위치를 X, Z 범위 -10에서 10 사이로 랜덤화
             lover.localPosition = new Vector3(
            Random.Range(-10f, 10f), // X축 범위
            lover.localPosition.y,   // Y축 유지
            Random.Range(-10f, 10f) // Z축 범위
           

        );
        dis = Vector3.Distance(lover.localPosition,runner.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 네모의 현재 위치를 관찰
        sensor.AddObservation(runner.localPosition.x);
        sensor.AddObservation(runner.localPosition.z);

        // 목표의 현재 위치를 관찰
    
        sensor.AddObservation(lover.localPosition.x);
        sensor.AddObservation(lover.localPosition.z);
        sensor.AddObservation(dis); //lover와 자신 사이의 거리를 관찰함
        
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // 행동 값 받기 (0:전진, 1: 후진, 2: 좌, 3: 우)
        int action = actionBuffers.DiscreteActions[0];

        Vector3 move = Vector3.zero;
        switch (action)
        {
            case 0: // 전진
                move = Vector3.forward * speed * Time.deltaTime;
                break;
            case 1: // 후진
                move = Vector3.back * speed * Time.deltaTime;
                break;
            case 2: // 왼쪽
                move = Vector3.left * speed * Time.deltaTime;
                break;
            case 3: // 오른쪽
                move = Vector3.right * speed * Time.deltaTime;
                break;
        }

        // 이동 적용
        runner.localPosition += move;

        // 목표 도달 여부 확인
        if (dis < 1.0f)
        {
            SetReward(1f); // 목표 도달 시 보상
            EndEpisode(); // 에피소드 종료
        }
        else if (1.0f<dis&&dis<3)
        {
           AddReward(0.1f);

        }
        else if(dis>10)
        {
            AddReward(-1);
        }
        else
        {
           AddReward(-0.01f);
        }
      
        
        
        if(GetCumulativeReward() < 0.0f)
        {
            EndEpisode();
        }
       

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 수동 제어를 위한 로직
        var discreteActions = actionsOut.DiscreteActions;
      

        if (Input.GetKey(KeyCode.W)) discreteActions[0] = 0; // 전진
        if (Input.GetKey(KeyCode.S)) discreteActions[0] = 1; // 후진
        if (Input.GetKey(KeyCode.A)) discreteActions[0] = 2; // 왼쪽
        if (Input.GetKey(KeyCode.D)) discreteActions[0] = 3; // 오른쪽
    }
}
