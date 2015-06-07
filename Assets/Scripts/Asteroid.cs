using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour {

    public float speed;
    public float rotationSpeed;
    public bool DivideAfterHit;
    public int DivideTimes;
    public GameObject AsteroidPerfab;
    public int Score;

    bool isAlive;
    int points;
    Vector3 rotationAxis;

    void OnEnable()
    {
        rotationAxis.x = 1;   
    }

	// Asteroid initialization: we set one of random shapes and random rotation
	void Start () 
    {
        GetComponent<Rigidbody2D>().AddForce(Random.insideUnitSphere * speed, ForceMode2D.Impulse);
        GetComponent<Rigidbody2D>().AddTorque(rotationSpeed, ForceMode2D.Impulse);
        isAlive = true;
	}
	

    void Update()
    {

    }

    public void SetAsteroidSize(float multiplier)
    {
        transform.localScale = new Vector3(transform.localScale.x * multiplier, transform.localScale.y * multiplier, 1.0f);
    }


    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.tag == "PlayerBullet" || c.gameObject.tag == "Player")
        {            
            if (DivideAfterHit && isAlive)
            {
                InstantiateChildAsteroid(-15);
                InstantiateChildAsteroid(15);                
            }
            GameMode gameMode = FindObjectOfType<GameMode>();
            if (gameMode)
            {
                gameMode.RemoveAsteroid(Score);
            }
            isAlive = false;
        }
    }

    void InstantiateChildAsteroid(float direction)
    {
        GameObject childAsteroidGO = (GameObject)Instantiate(AsteroidPerfab, transform.position, Quaternion.identity);
        childAsteroidGO.GetComponent<Asteroid>().SetAsteroidSize(0.5f);
        childAsteroidGO.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, direction) * GetComponent<Rigidbody2D>().velocity;
        var childAsteroid = childAsteroidGO.GetComponent<Asteroid>();
        if (childAsteroid)
        {
            childAsteroid.DivideTimes = DivideTimes - 1;
            childAsteroid.DivideAfterHit = childAsteroid.DivideTimes != 0;
            childAsteroid.Score = Score * 2;
        }
        GameMode gameMode = FindObjectOfType<GameMode>();
        if (gameMode)
        {
            gameMode.AddAsteroid();
        }
    }

    void LateUpdate()
    {
        if (!isAlive)
        {
            gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            Destroy(gameObject);
            //Destroy(gameObject, gameObject.GetComponent<AudioSource>().clip.length);
        }
    }
}
