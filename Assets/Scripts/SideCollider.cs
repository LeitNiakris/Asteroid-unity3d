using UnityEngine;
using System.Collections;

public class SideCollider : MonoBehaviour {
    public int XMultiplier;
    public int YMultiplier;
    public GameObject AsteriodPrefab;

	void Start () {
	
	}
	
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //if this object's trigger collider hits another object tagged "scenery"...
        if (other.tag == "Asteroid" || other.tag == "Player" || other.tag == "PlayerBullet")
        {
            //...get the other object's position...
            Vector3 pos = other.transform.position;
            //...and spawn it a little closer to screen center, not to get in touch with opposite SideCollider
            if (XMultiplier != 0)
                pos.x = -pos.x + XMultiplier * other.bounds.size.x / 2;
            if (YMultiplier != 0)
                pos.y = -pos.y + YMultiplier * other.bounds.size.y / 2;
            other.transform.position = pos;
        }
    }
}
