using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float LifeTime;

    private float alreadyAlive;
	// Use this for initialization
	void Start () 
    {
        alreadyAlive = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        alreadyAlive += Time.deltaTime;
        if (alreadyAlive > LifeTime)
        {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter2D(Collision2D c)
    {
        Destroy(gameObject);
    }
}
