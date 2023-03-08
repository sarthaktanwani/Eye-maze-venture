// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO.Ports;
// using System.Threading;
// using System;

// //<summary>
// //Ball movement controlls and simple third-person-style camera
// //</summary>
// public class RollerBallMod : MonoBehaviour {

// 	enum eye_state
//     {
//         IDLE,
//         LOW_THRES,
//         HIGH_THRES,
//         BLINK,
//         LEFT_MOVT,
//         RIGHT_MOVT
//     };
//     static eye_state horz_state, vert_state;
//     public float LerpSpeed;
//     const int HIGH_THRESHOLD = 570;
//     const int LOW_THRESHOLD = 450;
//     const int BLINK_THRESHOLD = 650;

//     static bool _continue;
//     static SerialPort _serialPort;
//     static string name;
//     static long last_time_in_low = 100,last_time_in_high = 100, i = 1;
//     //Resource on coroutines: https://www.youtube.com/watch?v=t4m8pmahbzY
//     private Coroutine moveCoroutine;
//     private WaitForSeconds wait = new WaitForSeconds(10.0f);  
    
//     //https://forum.unity.com/threads/measuring-time-passed-in-seconds.427611/  
//     static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
//     static System.Diagnostics.Stopwatch idleTime = new System.Diagnostics.Stopwatch();
// 	//****************************************************************//
    
// 	public GameObject ViewCamera = null;
// 	public AudioClip JumpSound = null;
// 	public AudioClip HitSound = null;
// 	public AudioClip CoinSound = null;

// 	private Rigidbody mRigidBody = null;
// 	private AudioSource mAudioSource = null;
// 	private bool mFloorTouched = false;

// 	void Start () {

// 		Thread readThread = new Thread(Read);

//         // Create a new SerialPort object with default settings.
//         _serialPort = new SerialPort("/dev/ttyACM0", 115200);

//         // Set the read/write timeouts
//         _serialPort.ReadTimeout = 600;
//         // _serialPort.WriteTimeout = 500;

//         _serialPort.Open();
//         _continue = true;
//         // StartCoroutine(Read());
//         readThread.Start();

// 		mRigidBody = GetComponent<Rigidbody> ();
// 		mAudioSource = GetComponent<AudioSource> ();
// 	}

// 	void FixedUpdate(int z) {
// 		// Debug.Log("In FixedUpdate");
// 		if (mRigidBody != null) {

// 			if(Input.GetButtonDown("Fire1")) {
// 				mRigidBody.AddForce(Vector3.left * 400);
// 				// mRigidBody.AddTorque(Vector3.left * 200);
// 			}
// 			if(Input.GetButtonDown("Fire2")) {
// 				mRigidBody.AddForce(Vector3.right * 400);
// 				// mRigidBody.AddTorque(Vector3.right * 200);
// 			}

// 			// if(watch.IsRunning == false)
// 			// {
// 			// 	watch.Start();
// 			// }
// 			// if(watch.ElapsedMilliseconds >= 500)
// 			if(!(watch.IsRunning && watch.ElapsedMilliseconds <= 500))
// 			{
// 				watch.Stop();
// 				watch.Reset();
// 				// watch.Start();
// 				if(horz_state == eye_state.BLINK)
// 				{
// 					if(mAudioSource != null && JumpSound != null){
// 						mAudioSource.PlayOneShot(JumpSound);
// 					}
// 					mRigidBody.AddForce(Vector3.up*1000);
// 					// i *= (-1);
// 					// if(moveCoroutine != null)
// 					// 	StopCoroutine(moveCoroutine);
// 					// moveCoroutine = StartCoroutine(DoMove(transform.position + new Vector3(0, 0, 10*i)));

// 					Debug.Log("BLINK DETECTED");
// 					horz_state = eye_state.IDLE;
// 					watch.Start();
// 				}
// 				else if(horz_state == eye_state.LEFT_MOVT)
// 				{
// 					mRigidBody.AddForce(Vector3.left * 400);
// 					// mRigidBody.AddForce(Vector3.forward * 400);
// 					// if(moveCoroutine != null)
// 					// 	StopCoroutine(moveCoroutine);
// 					// moveCoroutine = StartCoroutine(DoMove(transform.position + new Vector3(-10, 0, 0)));
	
// 					Debug.Log("LEFT MOTION DETECTED");
// 					// Debug.Log("UP MOTION DETECTED");
// 					horz_state = eye_state.IDLE;
// 					watch.Start();
// 				}
// 				else if(horz_state == eye_state.RIGHT_MOVT)
// 				{
// 					mRigidBody.AddForce(Vector3.right * 400);
// 					// mRigidBody.AddForce(Vector3.back * 400);
// 					// if(moveCoroutine != null)
// 					// 	StopCoroutine(moveCoroutine);
// 					// moveCoroutine = StartCoroutine(DoMove(transform.position + new Vector3(10, 0, 0)));

// 					Debug.Log("RIGHT MOTION DETECTED");
// 					// Debug.Log("DOWN MOTION DETECTED");
// 					horz_state = eye_state.IDLE;
// 					watch.Start();
// 				}
// 				// Debug.Log("Passed");
//         	}

// 			// if(Input.GetButtonDown("Fire1")) {
// 			// 	mRigidBody.AddForce(Vector3.left * 400);	
// 			// 	// mRigidBody.AddTorque(Vector3.left * 200);
// 			// }
// 			// if(Input.GetButtonDown("Fire2")) {
// 			// 	mRigidBody.AddForce(Vector3.right * 400);
// 			// 	// mRigidBody.AddTorque(Vector3.right * 200);
// 			// }
// 			// if(Input.GetButtonDown("Fire3")) {
// 			// 	mRigidBody.AddForce(Vector3.forward * 400);
// 			// 	// mRigidBody.AddTorque(Vector3.forward * 200);
// 			// }
// 			// if (Input.GetButton ("Horizontal")) {
// 			// 	// mRigidBody.AddForce(Vector3.back * Input.GetAxis("Horizontal") * 20);
// 			// 	mRigidBody.AddTorque(Vector3.back * Input.GetAxis("Horizontal")*200);
// 			// }
// 			// if (Input.GetButton ("Vertical")) {
// 			// 	// mRigidBody.AddForce(Vector3.right * 20);
// 			// 	mRigidBody.AddTorque(Vector3.right * Input.GetAxis("Vertical")*200);
// 			// }
// 			// if (Input.GetButtonDown("Jump")) {
// 			// 	if(mAudioSource != null && JumpSound != null){
// 			// 		mAudioSource.PlayOneShot(JumpSound);
// 			// 	}
// 			// 	mRigidBody.AddForce(Vector3.up*200);
// 			// }
// 		}
// 		if (ViewCamera != null) {
// 			Vector3 direction = (Vector3.up*2+Vector3.back)*2;
// 			RaycastHit hit;
// 			Debug.DrawLine(transform.position,transform.position+direction,Color.red);
// 			if(Physics.Linecast(transform.position,transform.position+direction,out hit)){
// 				ViewCamera.transform.position = hit.point;
// 			}else{
// 				ViewCamera.transform.position = transform.position+direction;
// 			}
// 			ViewCamera.transform.LookAt(transform.position);
// 		}
// 	}

// 	void OnCollisionEnter(Collision coll){
// 		if (coll.gameObject.tag.Equals ("Floor")) {
// 			mFloorTouched = true;
// 			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.y > .5f) {
// 				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
// 			}
// 		} else {
// 			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.magnitude > 2f) {
// 				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
// 			}
// 		}
// 	}

// 	void OnCollisionExit(Collision coll){
// 		if (coll.gameObject.tag.Equals ("Floor")) {
// 			mFloorTouched = false;
// 		}
// 	}

// 	void OnTriggerEnter(Collider other) {
// 		if (other.gameObject.tag.Equals ("Coin")) {
// 			if(mAudioSource != null && CoinSound != null){
// 				mAudioSource.PlayOneShot(CoinSound);
// 			}
// 			Destroy(other.gameObject);
// 		}
// 	}

// 	public static void Read()
//     {
//         while (_continue)
//         {
//             try
//             {
//                 string message = _serialPort.ReadLine();
// 				int len = message.Split('\t').Length;
// 				// Debug.Log(len);
// 				if(len == 8)
// 				{
// 					int horz_val = Int16.Parse(message.Split('\t')[2]);
// 					int vert_val = Int16.Parse(message.Split('\t')[4]);
// 					// int horz_val = 500;
// 					// Debug.Log(val);
// 					// if(time_in_low.IsRunning && val > LOW_THRESHOLD)
// 					// {
// 					//     last_time_in_low = time_in_low.ElapsedMilliseconds;
// 					//     // Console.WriteLine("Time in LOW was: " + last_time_in_low + " ms");
// 					//     time_in_low.Stop();
// 					//     time_in_low.Reset();
// 					// }
// 					// if(time_in_high.IsRunning && val < HIGH_THRESHOLD)
// 					// {
// 					//     last_time_in_high = time_in_high.ElapsedMilliseconds;
// 					//     // Console.WriteLine("Time in LOW was: " + last_time_in_low + " ms");
// 					//     time_in_high.Stop();
// 					//     time_in_high.Reset();
// 					// }
// 					if(horz_val > BLINK_THRESHOLD)
// 					{
// 						updateHorzState(3);
// 					}
// 					else if(horz_val > HIGH_THRESHOLD && horz_state != eye_state.HIGH_THRES && horz_state != eye_state.BLINK)
// 					{
// 						// time_in_high.Start();
// 						updateHorzState(1);
// 					}
// 					else if(horz_val < LOW_THRESHOLD && horz_state != eye_state.LOW_THRES && horz_state != eye_state.BLINK)
// 					{
// 						// time_in_low.Start();
// 						updateHorzState(0);
// 					}
// 					else
// 					{
// 						if(idleTime.IsRunning == false)
// 						{
// 							idleTime.Start();
// 						}

// 						if(idleTime.ElapsedMilliseconds > 1000)
// 						{
// 							idleTime.Reset();
// 							idleTime.Stop();
// 							// if((horz_state == eye_state.LOW_THRES && last_time_in_low < 50) || (horz_state == eye_state.HIGH_THRES && last_time_in_high < 50))
// 							//     horz_state = eye_state.BLINK;
// 							// else
// 							horz_state = eye_state.IDLE;
// 						}
// 					}

// 					if(vert_val > BLINK_THRESHOLD)
// 					{
// 						updateState(3);
// 					}
// 					else if(vert_val > HIGH_THRESHOLD && vert_val != eye_state.HIGH_THRES && vert_val != eye_state.BLINK)
// 					{
// 						// time_in_high.Start();
// 						updateState(1);
// 					}
// 					else if(vert_val < LOW_THRESHOLD && vert_val != eye_state.LOW_THRES && vert_val != eye_state.BLINK)
// 					{
// 						// time_in_low.Start();
// 						updateState(0);
// 					}
// 					else
// 					{
// 						if(idleTime.IsRunning == false)
// 						{
// 							idleTime.Start();
// 						}

// 						if(idleTime.ElapsedMilliseconds > 1000)
// 						{
// 							idleTime.Reset();
// 							idleTime.Stop();
// 							// if((vert_val == eye_state.LOW_THRES && last_time_in_low < 50) || (vert_val == eye_state.HIGH_THRES && last_time_in_high < 50))
// 							//     vert_val = eye_state.BLINK;
// 							// else
// 							vert_val = eye_state.IDLE;
// 						}
// 					}
// 				}
//             }
//             catch (TimeoutException) { }
//         }
//     }

// 	public static void updateHorzState(int cross_thres)
//     {
//         switch (horz_state)
//         {
//             case eye_state.IDLE:
//             {
//                 if(cross_thres == 0)
//                 {
//                     // Debug.Log("ID->LT;");
//                     horz_state = eye_state.LOW_THRES;
//                 }
//                 else if(cross_thres == 1)
//                 {
//                     // Debug.Log("ID->HT;");
//                     horz_state = eye_state.HIGH_THRES;
//                 }
//                 else if(cross_thres == 3)
//                 {
//                     // Debug.Log("ID->B;");
//                     horz_state = eye_state.BLINK;
//                 }
//             }
//                 break;
//             case eye_state.LOW_THRES:
//             {
//                 if(cross_thres == 1)
//                 {
//                     // Debug.Log("LT->R;");
//                     horz_state = eye_state.RIGHT_MOVT;
//                 }
//                 else
//                 {
//                     // Debug.Log("LT->ID;");
//                     horz_state = eye_state.IDLE;
//                 }
//             }
//                 break;
            
//             case eye_state.HIGH_THRES:
//             {
//                 if(cross_thres == 0)
//                 {
//                     // Debug.Log("HT->L;");
//                     horz_state = eye_state.LEFT_MOVT;
//                 }
//                 else if(cross_thres == 3)
//                 {
//                     Console.Write("HT->B;");
//                     horz_state = eye_state.BLINK;
//                 }
//                 else
//                 {
//                     // Debug.Log("HT->ID;");
//                     horz_state = eye_state.IDLE;
//                 }
//             }
//                 break;
//             default:
//                 break;
//         }
//     }
// 	public static void updateVertState(int cross_thres)
//     {
//         switch (horz_state)
//         {
//             case eye_state.IDLE:
//             {
//                 if(cross_thres == 0)
//                 {
//                     // Debug.Log("ID->LT;");
//                     horz_state = eye_state.LOW_THRES;
//                 }
//                 else if(cross_thres == 1)
//                 {
//                     // Debug.Log("ID->HT;");
//                     horz_state = eye_state.HIGH_THRES;
//                 }
//                 else if(cross_thres == 3)
//                 {
//                     // Debug.Log("ID->B;");
//                     horz_state = eye_state.BLINK;
//                 }
//             }
//                 break;
//             case eye_state.LOW_THRES:
//             {
//                 if(cross_thres == 1)
//                 {
//                     // Debug.Log("LT->R;");
//                     horz_state = eye_state.RIGHT_MOVT;
//                 }
//                 else
//                 {
//                     // Debug.Log("LT->ID;");
//                     horz_state = eye_state.IDLE;
//                 }
//             }
//                 break;
            
//             case eye_state.HIGH_THRES:
//             {
//                 if(cross_thres == 0)
//                 {
//                     // Debug.Log("HT->L;");
//                     horz_state = eye_state.LEFT_MOVT;
//                 }
//                 else if(cross_thres == 3)
//                 {
//                     Console.Write("HT->B;");
//                     horz_state = eye_state.BLINK;
//                 }
//                 else
//                 {
//                     // Debug.Log("HT->ID;");
//                     horz_state = eye_state.IDLE;
//                 }
//             }
//                 break;
//             default:
//                 break;
//         }
//     }
    

// }
