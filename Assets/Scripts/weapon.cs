using UnityEngine;
using System.Collections;

public class weapon : MonoBehaviour {

	public enum fireType
	{
		SINGLE = 0,
		BURST,
		AUTO,
		SHOTGUN
	}

	Rigidbody2D rb;
	public GameObject proj;

	//for modification for other weapons
	Transform fireLocation;
	public fireType weaponFireType;
	//reload Stuff
	bool reloading;
	public float reloadTime = 1.0f;
	public float curTime = 0f;
	int burstNum = 8;
	int curBurstNum =0;
	// Use this for initialization
	void Start () {
		reloading = false;
		rb = GetComponent<Rigidbody2D>();
		fireLocation = transform.FindChild("firePoint");
	}
	
	// Update is called once per frame
	void Update () {

		if(transform.parent != null)
		{
			rb.isKinematic = true;
		}
		else
		{
			rb.isKinematic = false;
		}

		if(reloading)// && weaponFireType != fireType.BURST)
		{
			curTime += Time.deltaTime;
			if(curTime >= reloadTime)
			{
				reloading = false;
				curTime = 0.0f;
			}
		}
		//if(reloading && weaponFireType == fireType.BURST)
		//{
				//if(curBurstNum < burstNum)
				//{
					//curBurstNum++;
					//reloading = false;
					//curTime = 0.0f;
				//}
				//else
				//{
				//	reloading = false;
				//	curBurstNum = 0;
				//}
		//}

		if( weaponFireType == fireType.BURST)
		{
			if(Input.GetButtonUp("Fire1"))
			{
				curBurstNum = 0;
			}
		}

	
	}

	public void Shoot()
	{
		if(weaponFireType == fireType.SINGLE || weaponFireType == fireType.AUTO)
		{
			if(!reloading)
			{
				Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
				GameObject projShot = Instantiate(proj,fireLocation.position,fireLocation.rotation) as GameObject;
				projShot.GetComponent<ProjectileNew>().dir = direction;

				if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
				{
					projShot.GetComponent<ProjectileNew>().faceLeft =true;
				}
				reloading = true;
			}
		}
		else if(weaponFireType == fireType.BURST)
		{
			if(curBurstNum < burstNum && !reloading)
			{
				Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
				GameObject projShot = Instantiate(proj,fireLocation.position,fireLocation.rotation) as GameObject;
				projShot.GetComponent<ProjectileNew>().dir = direction;
				
				if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
				{
					projShot.GetComponent<ProjectileNew>().faceLeft =true;
				}
				reloading = true;
			}
			curBurstNum++;
		}
		else if(weaponFireType == fireType.SHOTGUN)
		{
			if(!reloading)
			{
				Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
				GameObject projShot = Instantiate(proj,fireLocation.position,fireLocation.rotation) as GameObject;
				GameObject projShot2 = Instantiate(proj,fireLocation.position,fireLocation.rotation) as GameObject;
				GameObject projShot3 = Instantiate(proj,fireLocation.position,fireLocation.rotation) as GameObject;
				projShot.GetComponent<ProjectileNew>().dir = direction;
				direction.x-=0.5f;
				direction.y-=0.5f;
				projShot2.GetComponent<ProjectileNew>().dir = direction;
				direction.x+=1f;
				direction.y+=1f;
				projShot3.GetComponent<ProjectileNew>().dir = direction;

				if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
				{
					projShot.GetComponent<ProjectileNew>().faceLeft =true;
					projShot2.GetComponent<ProjectileNew>().faceLeft =true;
					projShot3.GetComponent<ProjectileNew>().faceLeft =true;
				}
				reloading = true;
			}
		}


	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "weapon")
		{
			Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(),other);
			Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(),other);
		}
//		if(other.tag == "projectile")
//		{
//			Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(),other);
//			Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(),other);
//		}
	}
}
