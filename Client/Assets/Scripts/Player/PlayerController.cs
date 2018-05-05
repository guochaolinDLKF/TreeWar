using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerController : MonoBehaviour
{
    Vector3 movement;
    Rigidbody playerRigidbody;
    Animator animator;

    // player移动速度
    public float speed = 6f;

    private float x;
    private float y;
    private float xSpeed = 2;
    private float ySpeed = 2;
    private Quaternion direct; // player direct

    // 判断是否落地
    private bool grounded = true;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded")) return;
        // keyboard move
        float h = Input.GetAxisRaw("Horizontal"); // A D
        float v = Input.GetAxisRaw("Vertical");   // W S
        transform.Translate(Vector3.forward * v * speed * Time.deltaTime, Space.Self); // W S 上 下
        transform.Translate(Vector3.right * h * speed * Time.deltaTime, Space.Self); // A D 左右
        transform.localEulerAngles = new Vector3(0, -90, 0);                                                // }
        float res = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
        animator.SetFloat("Forward", res);
        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded == true)
            {
                animator.SetBool("Jump", true);
                playerRigidbody.velocity += new Vector3(0, 5, 0); //添加加速度
                playerRigidbody.AddForce(Vector3.up * 50); //给刚体一个向上的力，力的大小为Vector3.up*mJumpSpeed
                grounded = false;
            }
        }
    }

    // 落地检测
    void OnCollisionEnter(Collision collision)
    {
        animator.SetBool("Jump", false);
        grounded = true;
    }

}
