using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fox : MonoBehaviour
{
    public GameObject boy; // 추적 대상
    public GameObject runner; // 레이캐스트 대상
    private NavMeshAgent nav;
    private Transform tr;
    private Rigidbody rb;

    public Vector3 force; // Smash 시 추가될 힘
    public float detectionRange = 5f; // 레이 감지 거리
    public LayerMask targetLayer; // 감지할 레이어
    private float dis; // boy와의 거리

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // boy와의 거리 계산
        dis = Vector3.Distance(tr.position, boy.transform.position);

        // boy와의 거리 조건에 따라 추적
        if (dis > 5)
        {
            Stalking();
        }

        // runner를 레이로 감지
        Ray ray = new Ray(tr.position, tr.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange, targetLayer))
        {
            if (hit.collider.gameObject == runner)
            {
                Smash(); // runner 감지 시 Smash 실행
            }
        }

        // 디버깅용 레이 시각화
        Debug.DrawRay(tr.position, tr.forward * detectionRange, Color.red);
    }

    // boy를 추적하며 빠르게 회전
    void Stalking()
    {
        // boy를 향해 NavMeshAgent로 이동
        nav.destination = boy.transform.position;

        // boy의 방향 계산
        Vector3 directionToBoy = (boy.transform.position - tr.position).normalized;

        // boy를 향해 일정 속도로 회전
        Quaternion targetRotation = Quaternion.LookRotation(directionToBoy);

        // 일정한 속도로 목표 방향으로 회전
        float rotationSpeed = 360f; // 초당 회전 속도
        tr.rotation = Quaternion.RotateTowards(tr.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Smash 동작
    void Smash()
    {
        if (rb != null)
        {
            // 전방으로 힘 추가
            rb.AddForce(tr.forward * force.magnitude, ForceMode.Impulse);
            Debug.Log("Smash executed!");
        }
    }
}
