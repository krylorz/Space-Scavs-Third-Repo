using UnityEngine;
using System.Collections;

public class ArmorGrubProjectile : MonoBehaviour {

	public Vector3 dir;
	Rigidbody2D rb;
	public bool faceLeft;
	public float speed = 5f;
	GameObject player;
	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>();
		
		if (faceLeft)
		{
			//do we really want this or should sprite facing be determined by weapon aim?
			Vector3 newscale = transform.localScale;
			newscale.x =-Mathf.Abs (transform.localScale.x);
			transform.localScale = newscale;
			Vector3 newRot = transform.rotation.eulerAngles;
			newRot.z = -newRot.z;
			transform.rotation= Quaternion.Euler(newRot);
			dir = new Vector2 (-1,1);
		}
		dir = new Vector2(1,1);
		//rb.velocity = dir.normalized * speed;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	void OnBecameInvisible()
	{
		Destroy(this.gameObject);
	}
	
	
	void OnCollisionEnter2D(Collision2D other)
	{
		print (other.gameObject.tag);
		//do not collide with another weapon
		if(other.gameObject.tag != "Enemy")
		{
			//do not collide with the weapons
			//these had to be separated because order of operations was messing up with them 
			//both in an or statement
			if(other.gameObject.tag != "weapon")
			{
				if( other.gameObject.tag == "Player")
				{
					player.GetComponent<Controls>().takeDamage();
					Destroy(this.gameObject);

				}
				Destroy(this.gameObject);
			}
		}

	}
	void OnCollisionExit2D(Collision2D other)
	{
		if(other.gameObject.tag == "Enemy")
		{
			rb.velocity = Vector2.zero;
			rb.velocity = dir.normalized * speed;
		}
	}
}
