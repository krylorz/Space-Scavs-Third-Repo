using UnityEngine;
using System.Collections;

public class ProjectileNew : MonoBehaviour {


	public Vector3 dir;
	Rigidbody2D rb;
	public bool faceLeft;
	public float speed = 3f;
	public GameObject flash;
	public GameObject explosion;
	// Use this for initialization
	void Start () 
	{

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

		}
		if(flash != null)
		{
			GameObject obj = Instantiate(flash,this.transform.position,this.transform.rotation) as GameObject;
			obj.transform.localScale = this.transform.localScale;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		rb.velocity = dir.normalized * speed;
	}

	void OnBecameInvisible()
	{
		if(explosion != null)
		{
			Instantiate(explosion,this.transform.position,this.transform.rotation);
		}
		Destroy(this.gameObject);
	}


	void OnCollisionEnter2D(Collision2D other)
	{

		//do not collide with another weapon
		if(other.gameObject.tag != "weapon")
		{
			//do not collide with the player
			//these had to be separated because order of operations was messing up with them 
			//both in an or statement
			if( other.gameObject.tag != "Player")
			{
				if(explosion != null)
				{
					Instantiate(explosion,this.transform.position,this.transform.rotation);
				}
				Destroy(this.gameObject);
			}
		}
	}
}
