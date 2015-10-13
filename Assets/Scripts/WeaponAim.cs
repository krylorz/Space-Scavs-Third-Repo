using UnityEngine;
using System.Collections;

public class WeaponAim : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		// get direction vector
		Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;

		//used to normalize, but its an unneccesary step
		//dir.Normalize();
		//print(dir);


		//if facing right
		if(this.transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
		{
			//just make the 'right' vector that direction
			this.transform.right = dir;
		}
		else // facing left
		{
			//since the arm does not go to the -1 scale like the body (albeit doesn't seem logical)
			// we have to negate the x direction or else we only get the positive side of the circle
			dir.x = dir.x*-1f;
			this.transform.right = dir;
		}

	}
}
