using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float LifeTime;

    private float alreadyAlive;

	void Start () 
    {
        alreadyAlive = 0.0f;
	}
	

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
