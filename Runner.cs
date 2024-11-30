using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Runner : Agent
{
    public Transform runner;
    public Transform lover;
    public float speed = 5.0f;
    private Rigidbody runnerRb;
    private float dis;

    public override void Initialize()
    {
        runnerRb = runner.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        runner.localPosition = new Vector3(Random.Range(-5.0f, 5.0f), 0.5f, Random.Range(-5.0f, 5.0f));
        lover.localPosition = new Vector3(Random.Range(-5.0f, 5.0f), 0.5f, Random.Range(-5.0f, 5.0f));
        dis = Vector3.Distance(lover.localPosition, runner.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(runner.localPosition.x);
        sensor.AddObservation(runner.localPosition.z);
        sensor.AddObservation(lover.localPosition.x);
        sensor.AddObservation(lover.localPosition.z);
        sensor.AddObservation(dis);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        dis = Vector3.Distance(lover.localPosition, runner.localPosition);

        Vector3 move = Vector3.zero;
        switch (actionBuffers.DiscreteActions[0])
        {
            case 0: move = Vector3.forward * speed; break;
            case 1: move = Vector3.back * speed; break;
            case 2: move = Vector3.left * speed; break;
            case 3: move = Vector3.right * speed; break;
        }

        runnerRb.MovePosition(runner.localPosition + move * Time.fixedDeltaTime);

        if (dis < 1.0f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (dis < 3.0f)
        {
            AddReward(0.1f);
        }
        else if (dis > 10.0f)
        {
            AddReward(-1.0f);
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
        else discreteActions[0] = -1;
    }
}
