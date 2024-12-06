using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;



public class Runner : Agent
{
    public Transform runner;
    public Transform boy;
    public float speed = 5.0f;
   
    private Rigidbody runnerRb;
    
    private float dis;
    
    public override void Initialize()
    {
        runnerRb = runner.GetComponent<Rigidbody>();
     
        
      
    }

    public override void OnEpisodeBegin()
    {
        runner.transform.localPosition = new Vector3(Random.Range(-5, 5),0,Random.Range(-5, 5));
        boy.transform.localPosition = new Vector3(Random.Range(6, 10),0,Random.Range(6, 10));
        dis = Vector3.Distance(boy.localPosition, runner.localPosition);
        
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(runner.localPosition.x);
        sensor.AddObservation(runner.localPosition.z);
        sensor.AddObservation(boy.localPosition.x);
        sensor.AddObservation(boy.localPosition.z);
        sensor.AddObservation(dis);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        dis = Vector3.Distance(boy.localPosition, runner.localPosition);

        Vector3 move = Vector3.zero;
        switch (actionBuffers.DiscreteActions[0])
        {
            case 0: move = Vector3.forward; break;
            case 1: move = Vector3.back; break;
            case 2: move = Vector3.left; break;
            case 3: move = Vector3.right; break;
        }

        transform.Translate(move.normalized*speed*Time.deltaTime);

        if(dis<=1.0f)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        else if(dis<=5.0f)
        {
            AddReward(0.005f);

        }
        else if(dis>10.0f)

        {   
            AddReward(-0.5f);
            EndEpisode();
          
        }
        else 
        {
          AddReward(-0.001f);
        }
        
      
        
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W)) discreteActions[0] = 0;
        else if (Input.GetKey(KeyCode.S)) discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.A)) discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.D)) discreteActions[0] = 3;
        else discreteActions[0] = 0;
    }
}
