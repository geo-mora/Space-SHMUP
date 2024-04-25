using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Set in Inspector: Enemy")]
	public float speed = 10f; // The speed in m/s
	public float fireRate = 0.3f; // Seconds/shot (Unused)
	public float health = 10;
	public int score = 100; // Points earned for destroying this

    protected BoundsCheck bndCheck;

    void Awake() {
        bndCheck = GetComponent<BoundsCheck>();
    }

	// This is a Property: A method that acts like a field
	public Vector3 pos { // a
		get {
			return( this.transform.position );
	} set {
		this.transform.position = value;
	}
	}
	
	void Update() {
		Move();

        if (bndCheck != null && !bndCheck.isOnScreen) {
            if (pos.y < bndCheck.camHeight - bndCheck.radius) {
                Destroy(gameObject);
            }
        }
	}
	
	public virtual void Move() { // b
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}
	
	void OnCollisionEnter( Collision coll ) {
		GameObject otherGo = coll.gameObject;
		if ( otherGo.tag == "ProjectileHero" ) {
			Destroy(otherGo); // destroy the projectile
			Destroy(gameObject); // destroy this enemy game object
		}
		else {
			print("Enemy hit by non-projectileHero: " + otherGo.name);
		}
	}
}
