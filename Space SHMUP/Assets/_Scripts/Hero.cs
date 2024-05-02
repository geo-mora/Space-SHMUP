using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
	static public Hero S; // Singleton // a
	[Header("Set in Inspector")]

	// These fields control the movement of the ship
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;
	public float gameRestartDelay = 2f;
	public GameObject projectilePrefab;
	public float projectileSpeed = 40;

	[Header("Set Dynamically")]
	[SerializeField]
	public float _shieldLevel = 1;
	private GameObject lastTriggerGo = null;
	
	// Declare a new delegate type WeaponFireDelegate
	public delegate void WeaponFireDelegate(); // a
	// Create a WeaponFireDelegate field named fireDelegate.
	public WeaponFireDelegate fireDelegate;

	void Awake() {
	if (S == null) {
		S = this; // Set the Singleton // a
	} else {
		//Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
	}
	fireDelegate += TempFire;
	
	}
	
void Update () {
	// Pull in information from the Input class
	float xAxis = Input.GetAxis("Horizontal"); // b
	float yAxis = Input.GetAxis("Vertical"); // b

	// Change transform.position based on the axes
	Vector3 pos = transform.position;
	pos.x += xAxis * speed * Time.deltaTime;
	pos.y += yAxis * speed * Time.deltaTime;
	transform.position = pos;
	
	// Rotate the ship to make it feel more dynamic // c
	transform.rotation = Quaternion.Euler(yAxis*pitchMult,xAxis*rollMult,0);
		
	if (Input.GetKeyDown(KeyCode.Space)){
		TempFire();
		}
		
	// Use the fireDelegate to fire Weapons
	// First, make sure the button is pressed: Axis("Jump")
	// Then ensure that fireDelegate isn't null to avoid an error
	if (Input.GetAxis("Jump") == 1 && fireDelegate != null) { // d
		fireDelegate(); // e
	}
		
	}
	
	void TempFire(){
		GameObject projGO = Instantiate<GameObject>(projectilePrefab);
		projGO.transform.position = transform.position;
		Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
		// rigidB.velocity = Vector3.up * projectileSpeed;
		Projectile proj = projGO.GetComponent<Projectile>(); // h
		proj.type = WeaponType.blaster;
		float tSpeed = Main.GetWeaponDefinition( proj.type ).velocity;
		rigidB.velocity = Vector3.up * tSpeed;
	}
	
	void OnTriggerEnter(Collider other){
		Transform rootT = other.gameObject.transform.root;
		GameObject go = rootT.gameObject;
		//print("Triggered: "+go.name);
		//print("Triggered: " + other.gameObject.name);
		if (go == lastTriggerGo){
			return;
		}
		lastTriggerGo = go;
		if (go.tag == "Enemy"){
			shieldLevel--;
			Destroy(go);
		} else{
			print("Triggered by non-Enemy: "+go.name);
		}
	}
	
	public float shieldLevel{
		get {
			return( _shieldLevel );
		}
		set {
			_shieldLevel = Mathf.Min(value, 4);
			if (value < 0){
			   Destroy(this.gameObject);
			   Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}

}
