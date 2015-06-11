using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    public float EngineForce;
    public float RotationSpeed;
    public float MaxVelocity;

    private Player player;

	void Start () 
    {
        player = gameObject.GetComponent<Player>();
	}
	
	void Update () {
	
	}

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward * ((-1) * CrossPlatformInputManager.GetAxis("Horizontal") * RotationSpeed * Time.deltaTime));
        if (CrossPlatformInputManager.GetButtonDown("Space"))
        {
            player.Shoot();
        }

        if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
        {

            player.RigidBody.AddForce(transform.up * EngineForce * Time.deltaTime);
        }

        float curr_velocity_mag = player.RigidBody.velocity.magnitude;
        if (curr_velocity_mag > MaxVelocity)
        {
            Vector2 curr_velocity = player.RigidBody.velocity;
            player.RigidBody.velocity = curr_velocity.normalized * MaxVelocity;
        }
    }
}
