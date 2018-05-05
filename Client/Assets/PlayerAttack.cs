using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //public GameObject arrowPrefab;
    //private Animator anim;
    private Transform leftHandTrans;
    private Vector3 shootDir;
    private PlayerManager playerMng;
    [SerializeField]
    private Animator m_Anim;
    // Use this for initialization
    void Start()
    {
        leftHandTrans = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            if (Input.GetMouseButtonDown(0))
            {
               
                m_Anim.SetTrigger("Attack");
                Shoot();

            }
        }
    }
    public void SetPlayerMng(PlayerManager playerMng)
    {
        this.playerMng = playerMng;
    }
    private void Shoot()
    {
        GameFacade.Instance.m_ResMgr.Load(ResourceType.Prefab, "Arrow_Blue", leftHandTrans,Vector3.left);

        //transform.rotation = Quaternion.LookRotation(shootDir);
        //m_Anim.SetTrigger("Attack");
        //Invoke("Shoot", 0.1f);
        //playerMng.Shoot(arrowPrefab, leftHandTrans.position, Quaternion.LookRotation(shootDir));
    }
}
