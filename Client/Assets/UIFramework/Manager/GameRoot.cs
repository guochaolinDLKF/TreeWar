 using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour {

	// Use this for initialization
	void Start () {
       GameFacade.Instance.InitManager();
       //GameFacade.Instance.m_AudioMgr.PlayBgAudio(AudioKind.Bg_moderate);
	   

	}
}
