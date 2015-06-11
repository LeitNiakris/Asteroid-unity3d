using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class GameMode : MonoBehaviour {

    public static GameMode Current;			//A public static reference to itself (game mode should be singleton)
    public GameObject PlayerPrefab;			//The player ship
    public GameObject PlayerLifePrefab;		//The player ship
    public GameObject AsteroidPrefab;		//the column game object
    public int AsteroidsToSpawn;            //The number of asteroids on the first level
    public int InitialAsteroidScore;        // Score for biggest asteroid (each lesser will give 2x scores)
    public int PlayerLives;

    private int level;                              //Current level
    private int score;                              //The current score
    private int highScore;							//The high score
    private int asteroidsOnCurrentLevel;            //Asteroids to spawn on current level
    private GameState gameState;
    private string highScoreKey = "highScore";		
    private Vector2 screenSize;
    private int totalAsteroids;                     //Total number of asteroids in the level
    private List<GameObject> playerLivesHUD;        //Lifes in UI
    private int playerCurrentLives;                 //Available lifes
    private float timeInDeath;
    private float respawnTime;
    private float timeInGameOver;                   //Delay for level reset in case of game over
    private float playerSafeDistance;               //Min distance to other objects to resurrect player

    private GUIText currentScoreText;
    private GUIText gameStateText;
    private float textBlinkingDelta;

    // Asteroids will be spawned in particular range around player start
    List<Vector2> xSpawnLimits;
    List<Vector2> ySpawnLimits;

    enum GameState
    {
        InMenu,
        SpawnAsteroids,
        GameInProgress,
        LifeLost,
        RespawnAsteroids,
        GameOver,
    }
    
    ///////////////////////////////////////////////////////////////////////////////// 
    /// Public functions
    ///////////////////////////////////////////////////////////////////////////////// 

    // Called when asteroid was spawned
    public void AddAsteroid()
    {
        totalAsteroids++;
    }

    // Called when asteroid was destroyed
    public void RemoveAsteroid(int scoreToAdd)
    {
        score += scoreToAdd;
        currentScoreText.text = score.ToString();
        totalAsteroids--;
        if (totalAsteroids == 0)
        {
            LevelFinished();
        }
    }

    public void PlayerDestroyed()
    {
        if (playerCurrentLives > 0)
        {
            //Player life objects are not destroyed, just marked as inactive
            playerLivesHUD[playerCurrentLives - 1].SetActive(false);            
            playerCurrentLives -= 1;
            if (playerCurrentLives > 0)
            {
                gameState = GameState.LifeLost;
            }
            else
            {
                gameStateText.text = "GAME OVER, PRESS SPACE TO RESET";
                gameStateText.enabled = true;
                gameState = GameState.GameOver;
            }
        }
        timeInDeath = 0.0f;
        
    }


    ///////////////////////////////////////////////////////////////////////////////// 
    /// Private functions
    /////////////////////////////////////////////////////////////////////////////////

    void Awake()
    {
        //Ensure that there is only one manager
        if (Current == null)
            Current = this;
        else
            Destroy(gameObject);
    }


	void Start() 
    {
        score = 0;
        respawnTime = 3;
        timeInDeath = 0;
        playerCurrentLives = PlayerLives;
        textBlinkingDelta = 0.5f;
        asteroidsOnCurrentLevel = AsteroidsToSpawn;
        screenSize = Camera.main.ViewportToScreenPoint(new Vector3(1, 1, 0));      
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        // Spawn limits are made to create safe area around player during game start
        xSpawnLimits = new List<Vector2>();
        ySpawnLimits = new List<Vector2>();

        xSpawnLimits.Add(new Vector2(0, 0.25f));
        xSpawnLimits.Add(new Vector2(0.75f, 1.0f));
        ySpawnLimits.Add(new Vector2(0, 0.25f));
        ySpawnLimits.Add(new Vector2(0.75f, 1.0f));

        var playerTransform = PlayerPrefab.GetComponent<Transform>();
        var playerRenderer = PlayerPrefab.GetComponent<SpriteRenderer>();
        playerSafeDistance = playerRenderer.bounds.size.y * playerTransform.localScale.y * 3;

        GameObject scoreTextObject = new GameObject("ScoreText");
        currentScoreText = scoreTextObject.AddComponent<GUIText>();
        currentScoreText.gameObject.transform.position = new Vector3(0.1f, 0.95f, 0.0f);
        currentScoreText.text = "0";

        GameObject gameStatTextObject = new GameObject("GameStateText");
        gameStateText = gameStatTextObject.AddComponent<GUIText>();
        gameStateText.gameObject.transform.position = new Vector3(0.5f, 0.9f, 0.0f);
        gameStateText.GetComponent<GUIText>().alignment = TextAlignment.Center;
        gameStateText.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
        InitLevel();
        SpawnLifeHUD();
	}
	

	void Update () 
    {
	    switch (gameState)
        {
            case GameState.LifeLost:
                CheckForRespawn();
                break;
            case GameState.GameOver:
                timeInGameOver += Time.deltaTime;
                break;
            default:
                break;
        }
	}

    void FixedUpdate()
    {
        if (CrossPlatformInputManager.GetButtonDown("Space"))
        {
            switch (gameState)
            {
                case GameState.InMenu:
                    {
                        SpawnPlayer();
                        CancelInvoke("BlinkStartGameText");
                        gameState = GameState.GameInProgress;
                        gameStateText.enabled = false;
                        currentScoreText.enabled = true;
                    }
                    break;
                case GameState.GameInProgress:
                    break;
                case GameState.GameOver:
                    {
                        if (timeInGameOver > 1.5)
                        {
                            ClearLevel();
                            ResetLevel();
                            InitLevel();
                        }
                    }
                    break;
                case GameState.LifeLost:
                    break;
                case GameState.RespawnAsteroids:
                    {
                        ClearLevel();
                        InitLevel();
                    }
                    break;
                default:
                    break;
            }

        }
    }

    void InitLevel()
    {
        totalAsteroids = asteroidsOnCurrentLevel;
        currentScoreText.enabled = false;
        gameState = GameState.InMenu;

        for (int i = 0; i < asteroidsOnCurrentLevel; i++)
        {
            SpawnAsteroid();
        }
        gameStateText.text = "PRESS SPACE TO START";
        if (!IsInvoking("BlinkStartGameText"))
        {
            InvokeRepeating("BlinkStartGameText", 0.0f, textBlinkingDelta);
        }
    }

    void ResetLevel()
    {
        score = 0;
        timeInGameOver = 0;
        currentScoreText.text = "0";
        asteroidsOnCurrentLevel = AsteroidsToSpawn;
        playerCurrentLives = PlayerLives;
        level = 0;
        
        for (int i = 0; i < playerLivesHUD.Count; i++)
        {
            playerLivesHUD[i].SetActive(true);
        }
    }

    // Checks if time for respawning passed and there are no asteroids
    // close enough to player swawn point
    void CheckForRespawn()
    {
        timeInDeath += Time.deltaTime;
        if (timeInDeath > respawnTime && IsSafeForRespawn())
        {
            SpawnPlayer();
            gameState = GameState.GameInProgress;
        }
    }

    bool IsSafeForRespawn()
    {
        
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        Vector3 respawnPosition = new Vector3(0.0f, 0.0f, 0.0f);
        for (int i = 0; i < asteroids.Length; i++)
        {
            Vector3 asteroidPosition = asteroids[i].transform.position;
            float distance = (asteroidPosition - respawnPosition).magnitude;
            if (distance < playerSafeDistance)
            {
                return false;
            }
        }
        return true;
    }

    void LoadHighScore()
    {
        //Reset the score and get the high score from the playerprefs
        score = 0;
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
    }


    void SpawnPlayer()
    {
        var spawnPointInWorldCoord = new Vector3(0.5f, 0.5f, -Camera.main.transform.position.z);
        var player = (GameObject)Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        player.SetActive(true);
    }

    void SpawnAsteroid()
    {
        var xArray = xSpawnLimits[Random.Range(0,2)];
        var yArray = ySpawnLimits[Random.Range(0,2)];
        var spawnPointInWorldCoord = new Vector3(Random.Range(xArray.x, xArray.y), Random.Range(yArray.x, yArray.y), -Camera.main.transform.position.z);
        var asteroidGO = (GameObject)Instantiate(AsteroidPrefab, Camera.main.ViewportToWorldPoint(spawnPointInWorldCoord), new Quaternion());
        asteroidGO.gameObject.SetActive(true);
        var asteroid = asteroidGO.GetComponent<Asteroid>();      
        if (asteroid)
        {
            // Initial asteroids will divide 2 times
            asteroid.DivideAfterHit = true;
            asteroid.DivideTimes = 2;
            asteroid.Score = 50;
        }
    }

    void SpawnLifeHUD()
    {
        var renderer = PlayerLifePrefab.GetComponent<SpriteRenderer>();
        var playerLifeTransform = PlayerLifePrefab.GetComponent<Transform>();
        if (renderer != null && playerLifeTransform != null)
        {           
            playerLivesHUD = new List<GameObject>(0);
            for (int i = 0; i < playerCurrentLives; i++)
            {
                float shift = i * renderer.bounds.size.x * playerLifeTransform.localScale.x;
                GameObject hudLife = (GameObject)GameObject.Instantiate(PlayerLifePrefab);
                Vector3 position = Camera.main.ViewportToWorldPoint(new Vector3(0.08f + shift, 0.9f, 0.0f));
                position.z = 0.0f;
                hudLife.transform.position = position;
                playerLivesHUD.Add(hudLife);
            }
        }
    }

    void LevelFinished()
    {
        level++;
        asteroidsOnCurrentLevel++;
        gameStateText.text = "LEVEL CLEARED";
        gameStateText.enabled = true;
        gameState = GameState.RespawnAsteroids;
    }

    void ClearLevel()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("PlayerBullet");
        for (int i = 0; i < bullets.Length; i++)
        {
            Destroy(bullets[i]);
        }

        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i]);
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }
    

    void BlinkStartGameText()
    {
        gameStateText.enabled = !gameStateText.enabled;
    }

}
