using UnityEngine;
using System.Collections;

public class SideCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //if this object's trigger collider hits another object tagged "scenery"...
        if (other.tag == "Asteroid" || other.tag == "Player" || other.tag == "PlayerBullet")
        {
            //...get the other object's position...
            Vector3 pos = other.transform.position;
            pos.x *= -1;
            pos.y *= -1;
            other.transform.position = pos;
        }
    }
}
