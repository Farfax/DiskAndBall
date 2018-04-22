using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainSceneController : MonoBehaviour {
	
	
	
	void Start () {
		
	}
	
	void Update () {
		
	}

	public void LoadLevel(string name)
	{
		SceneManager.LoadScene(name);
	}
}
