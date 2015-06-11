using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    public Rigidbody2D RigidBody;      //Player's RigidBody

    public Bullet BulletInstance;		//the bullet game object
    public float BulletSpeed;
    public int MaxBullets;

    private bool isAlive;
    private Vector3 bulletStartPositionShift;       //The bullet must flu out of ship's nose!

	void Start () {
        isAlive = true;
        var renderer = GetComponent<SpriteRenderer>();
        bulletStartPositionShift = new Vector3(0, renderer.bounds.size.y / 3, 0);
	}
	

	void Update () 
    {

	}


    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Asteroid" || c.gameObject.tag == "EnemyProjectile" || c.gameObject.tag == "EnemyShip")
        {
            DestroyShip();
        }
    }

    public void Shoot()
    {
        if (isAlive)
        {
            if (GameObject.FindGameObjectsWithTag("PlayerBullet").Length < MaxBullets)
            {
                
                var bulletStartPosition = Quaternion.Euler(transform.rotation.eulerAngles) * bulletStartPositionShift;
                Bullet p = (Bullet)GameObject.Instantiate(BulletInstance, transform.position + bulletStartPosition, transform.rotation);
                p.GetComponent<Rigidbody2D>().velocity = transform.up * BulletSpeed * Time.deltaTime;

            }
        }
    }

    void DestroyShip()
    {
        isAlive = false;
        GameMode gameMode = FindObjectOfType<GameMode>();
        if (gameMode)
        {
            gameMode.PlayerDestroyed();
        }
        Destroy(gameObject);

    }
}
