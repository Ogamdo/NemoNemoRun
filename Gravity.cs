using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    private Rigidbody rb; // Script가 적용될 Object의 Rigidbody
    private Transform tr; // Script가 적용될 Object의 Transform
    [SerializeField]  float checkDistance = 2.0f; // Raycast 거리
    [SerializeField]  LayerMask groundLayer; // 바닥 레이어

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        CheckGround();
    }

 

    void CheckGround()
    {
        // 캐릭터의 위치에서 아래로 Raycast 발사
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, checkDistance, groundLayer))
        {
            // Raycast가 바닥을 감지했을 때 조정
            Vector3 groundPosition = hit.point;
            groundPosition.y += rb.transform.localScale.y / 2; // 오브젝트 높이 조정

            // 캐릭터가 바닥보다 아래에 있을 때 위치 조정
            if (tr.position.y < groundPosition.y)
            {
                tr.position = groundPosition; // 오브젝트 위치 조정
            }
        }
    }
}