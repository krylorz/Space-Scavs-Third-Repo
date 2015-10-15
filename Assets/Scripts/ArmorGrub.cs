using UnityEngine;
using System.Collections;

public class ArmorGrub : MonoBehaviour {
		
	enum AI_State
	{
		ATTACK,
		HUNT,
		RETREAT,
		PATROL,
	}

	protected float ai_attack_range = 2f;
	protected float ai_retreat_range = 1f;
	protected float ai_max_movement = 20f;
	public GameObject player;
	protected float playerDist;
	protected bool playerOnRight;
	Rigidbody2D rb;
	float speed = 1f;
	public float maxSpeed;
	protected Vector3 startPoint;
	protected float distFromStart;
	public float spread;
	public int health;
	Animator ani;
	AI_State grubState;
	public GameObject projectile;
	public GameObject firePoint;
	float timer;
	float maxTimer = 0.55f;
	bool canShoot = true;
		
	// Use this for initialization
	void Start () {
		GetPlayer();
		rb = GetComponent<Rigidbody2D>();
		ani = GetComponent<Animator>();
		startPoint = this.transform.position;
	}
	
	void OnAwake()
	{
		grubState = AI_State.PATROL;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(timer <=0)
		{
			canShoot = true;
			timer = maxTimer;
		}

		if(!canShoot)
		{
			timer -=Time.deltaTime;
		}
		playerDist = Vector2.Distance((Vector2)transform.position,(Vector2)player.transform.position);
		distFromStart = Vector2.Distance((Vector2)transform.position,(Vector2)startPoint);
		ChangeState();
		HandleState();
		PlayerSide();
		
	}
	
	
	
	void GetPlayer()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	void PlayerSide()
	{
		
		Vector3 flipScale = transform.localScale;
		if( player.transform.position.x > transform.position.x)
		{
			playerOnRight = true;
			flipScale.x = Mathf.Abs (transform.localScale.x);
		}
		else
		{
			playerOnRight = false;
			flipScale.x= -Mathf.Abs (transform.localScale.x);
		}
		this.transform.localScale = flipScale;
	}
	
	void ChangeState()
	{
		if(playerDist < ai_attack_range && playerDist > ai_retreat_range)
		{
			grubState = AI_State.ATTACK;
		}
		
		else if(playerDist > ai_attack_range && distFromStart < ai_max_movement)
		{
			grubState = AI_State.HUNT;
		}
		
		else if(playerDist < ai_retreat_range)
		{
			grubState = AI_State.RETREAT;
		}
		else
		{
			grubState = AI_State.PATROL;
		}
	}
	
	void HandleState()
	{
		switch(grubState)
		{
		case AI_State.RETREAT:

			if(Mathf.Abs(rb.velocity.x) < maxSpeed)
			{	
				Vector2 force2D = this.transform.position - player.transform.position;
				rb.AddForce(force2D.normalized * speed, ForceMode2D.Impulse);
			}
			
			break;
		case AI_State.ATTACK:

			ani.Play ("Attack");
			if(canShoot)
			{
				Instantiate (projectile,firePoint.transform.position,Quaternion.identity);
				canShoot = false;
			}
			
			
			break;
		case AI_State.HUNT:

			if(Mathf.Abs (rb.velocity.x) < maxSpeed)
			{
				Vector2 force2D = player.transform.position - this.transform.position;
				rb.AddForce(force2D.normalized * speed, ForceMode2D.Impulse);
			}
			break;
		default:
			break;
		}
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.tag == "projectile")
		{
			ani.Play("Damaged");
			health--;
			
			if(health <= 0)
			{
				Destroy(this.gameObject);
			}
		}
	}
}
