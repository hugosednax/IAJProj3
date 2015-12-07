using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    GameObject player;
    float y;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        y = this.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
	    this.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
	}
}
