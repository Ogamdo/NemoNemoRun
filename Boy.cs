using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boy : MonoBehaviour
{
    private Rigidbody rb;
    public BoxCollider Filed;
    public float speed = 100;
    private bool turn = false;
    private NavMeshAgent nav;
    private Bounds bounds= new Bounds();
    public Vector3 patrollPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        bounds = Filed.bounds;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = speed;
        patrollPos = new Vector3(bounds.max.x, transform.position.y, bounds.max.z);
        nav.destination = patrollPos;
    }
    void Patroll()
    {
        
            if(turn)
            {
                patrollPos = new Vector3(bounds.min.x, transform.position.y,bounds.min.z);
                turn =!turn;
            }
            else
            {
                 patrollPos = new Vector3(bounds.max.x, transform.position.y, bounds.max.z);
            }
            
        
        
    }
    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        Patroll();
    }
}
