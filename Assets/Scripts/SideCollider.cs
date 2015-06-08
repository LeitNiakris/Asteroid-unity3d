using UnityEngine;
using System.Collections;

public class SideCollider : MonoBehaviour {
    public int XMultiplier;
    public int YMultiplier;
    public GameObject AsteriodPrefab;

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
            UnityEngine.Debug.Log(string.Format("Position before: x={0}, y={1}", other.transform.position.x, AsteriodPrefab.transform.position.y));
            Vector3 pos = other.transform.position;
            if (XMultiplier != 0)
                pos.x = -pos.x + XMultiplier * other.bounds.size.x / 2;
            if (YMultiplier != 0)
                pos.y = -pos.y + YMultiplier * other.bounds.size.y / 2;
            other.transform.position = pos;
            UnityEngine.Debug.Log(string.Format("Position after: x={0}, y={1}", other.transform.position.x, AsteriodPrefab.transform.position.y));
        }
    }
}
