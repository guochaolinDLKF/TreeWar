using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private Transform player;
    private Vector3 offset;//初始偏移
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;//使用标签来查找player，得到transform属性赋给player    
        offset = transform.position - player.position;//初始偏移等于当前位置减去玩家位置
    }
    void Update()
    {
        transform.position = player.position + offset;//当前位置等于玩家位置加上初始偏移
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);//得到注视Player的相机旋转角度
    }
}
