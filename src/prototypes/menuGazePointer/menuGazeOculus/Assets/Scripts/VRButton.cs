using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRButton : MonoBehaviour {

	public Image BackgroundImages;
	public Color NormalColor;
	public Color HighlightColor;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnGazeEnter(){
		BackgroundImages.color = HighlightColor;
	}

	public void OnGazeExit(){
		BackgroundImages.color = NormalColor;
	}
}
