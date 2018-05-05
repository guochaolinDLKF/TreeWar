using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public int m_Speed = 5;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        transform.Translate(Vector3.forward*m_Speed*Time.deltaTime);
	}
    void OnCollisionEnter(Collision collision)
    {
       Destroy(this.gameObject);
    }
}
