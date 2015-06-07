using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private bool isAlive;
    
    public float EngineForce;
    public float RotationSpeed;
    public float MaxVelocity;
    public int lives;
    public Rigidbody2D Rigid_body;

    public Bullet BulletInstance;		//the bullet game object
    public float BulletSpeed;
    public int MaxBullets;

	// Use this for initialization
	void Start () {
        isAlive = true;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isAlive)
        {
            transform.Rotate(Vector3.forward * ((-1) * Input.GetAxis("Horizontal") * RotationSpeed * Time.deltaTime));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
            }
        }
	}


    void FixedUpdate()
    {
        if (isAlive)
        {
            transform.Rotate(Vector3.forward * ((-1) * Input.GetAxis("Horizontal") * RotationSpeed * Time.deltaTime));
            if (Input.GetAxis("Vertical") > 0)
            {

                UnityEngine.Debug.Log(string.Format("Vertical input, {0}, {1}, {2}", transform.up, EngineForce, Time.deltaTime));
                Rigid_body.AddForce(transform.up * EngineForce * Time.deltaTime);
                Rigid_body.drag = 0.0f;
                //if (!gameObject.GetComponents<AudioSource>()[1].isPlaying)
                //{
                //    gameObject.GetComponents<AudioSource>()[1].Play();
                //}
            }
            else
            {
                Rigid_body.drag = 0.4f;
                //if (gameObject.GetComponents<AudioSource>()[1].isPlaying)
                //{
                //    gameObject.GetComponents<AudioSource>()[1].Stop();
                //}
            }
            float curr_velocity_mag = Rigid_body.velocity.magnitude;
            if (curr_velocity_mag > MaxVelocity)
            {
                Vector2 curr_velocity = Rigid_body.velocity;
                Rigid_body.velocity = curr_velocity.normalized * MaxVelocity;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "Asteroid" || c.gameObject.tag == "EnemyProjectile" || c.gameObject.tag == "EnemyShip")
        {
            DestroyShip();
        }
    }

    void Shoot()
    {
        if (GameObject.FindGameObjectsWithTag("PlayerBullet").Length < MaxBullets)
        {
            Bullet p = (Bullet)GameObject.Instantiate(BulletInstance, transform.position, transform.rotation);
            p.GetComponent<Rigidbody2D>().velocity = transform.up * BulletSpeed * Time.deltaTime;

            //gameObject.GetComponents<AudioSource>()[0].Play();

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
        //ParticleSystem ship_explosion_instance = (ParticleSystem)Instantiate(ship_explosion_prefab_, transform.position, transform.rotation);
        //ship_explosion_instance.gameObject.GetComponent<AudioSource>().Play();
    }
}
