using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

//<summary>
//Ball movement controlls and simple third-person-style camera
//</summary>
public class RollerBallMod : MonoBehaviour {

	enum c_states
    {
        IDLE,
        LOW_THRES,
        HIGH_THRES,
        BLINK,
        LEFT_MOVT,
        RIGHT_MOVT
    };
    static c_states eye_state;
    public float LerpSpeed;
    const int HIGH_THRESHOLD = 570;
	// const int HIGH_THRESHOLD = 555;
    const int LOW_THRESHOLD = 450;
	// const int LOW_THRESHOLD = 460;
    const int BLINK_THRESHOLD = 700;

    static bool _continue;
    static SerialPort _serialPort;
	public bool last_blink_F = false;
    static string name;
    static long last_time_in_low = 100,last_time_in_high = 100, i = 1;
    //Resource on coroutines: https://www.youtube.com/watch?v=t4m8pmahbzY
    private Coroutine moveCoroutine;
    private WaitForSeconds wait = new WaitForSeconds(10.0f);  
    
    //https://forum.unity.com/threads/measuring-time-passed-in-seconds.427611/  
    static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    static System.Diagnostics.Stopwatch idleTime = new System.Diagnostics.Stopwatch();
	//****************************************************************//
    
	public GameObject ViewCamera = null;
	public AudioClip JumpSound = null;
	public AudioClip HitSound = null;
	public AudioClip CoinSound = null;

	private Rigidbody mRigidBody = null;
	private AudioSource mAudioSource = null;
	private bool mFloorTouched = false;

	void Start () {

		Thread readThread = new Thread(Read);

        // Create a new SerialPort object with default settings.
        _serialPort = new SerialPort("/dev/ttyACM0", 115200);

        // Set the read/write timeouts
        _serialPort.ReadTimeout = 600;
        // _serialPort.WriteTimeout = 500;

        _serialPort.Open();
        _continue = true;
        // StartCoroutine(Read());
        readThread.Start();

		mRigidBody = GetComponent<Rigidbody> ();
		mAudioSource = GetComponent<AudioSource> ();
	}

	void FixedUpdate () {
		// Debug.Log("In FixedUpdate");
		if (mRigidBody != null) {

			if(Input.GetButtonDown("Fire1")) {
				mRigidBody.AddForce(Vector3.left * 100);
				// mRigidBody.AddForce(Vector3.forward * 400);
					Debug.Log("LEFT MOTION DETECTED");
				// mRigidBody.AddTorque(Vector3.left * 200);
			}
			if(Input.GetButtonDown("Fire2")) {
				mRigidBody.AddForce(Vector3.right * 100);
				// mRigidBody.AddForce(Vector3.back * 400);
					// Debug.Log("DOWN MOTION DETECTED");
					Debug.Log("RIGHT MOTION DETECTED");
				// mRigidBody.AddTorque(Vector3.right * 200);
			}

			// if(watch.IsRunning == false)
			// {
			// 	watch.Start();
			// }
			// if(watch.ElapsedMilliseconds >= 500)
			if(!(watch.IsRunning && watch.ElapsedMilliseconds <= 500))
			{
				watch.Stop();
				watch.Reset();
				// watch.Start();
				if(eye_state == c_states.BLINK)
				{
					idleTime.Stop();
					idleTime.Reset();
					if(last_blink_F == false)
					{	
						if(mAudioSource != null && JumpSound != null){
							mAudioSource.PlayOneShot(JumpSound);
						}
						last_blink_F = true;
						mRigidBody.AddForce(Vector3.up*1000);
						Debug.Log("BLINK DETECTED");
					}
					// i *= (-1);
					// if(moveCoroutine != null)
					// 	StopCoroutine(moveCoroutine);
					// moveCoroutine = StartCoroutine(DoMove(transform.position + new Vector3(0, 0, 10*i)));

					eye_state = c_states.IDLE;
					watch.Start();
				}
				else if(eye_state == c_states.LEFT_MOVT)
				{
					idleTime.Stop();
					idleTime.Reset();
					mRigidBody.AddForce(Vector3.forward * 400);
					// mRigidBody.AddForce(Vector3.left * 400);
					// if(moveCoroutine != null)
					// 	StopCoroutine(moveCoroutine);
					// moveCoroutine = StartCoroutine(DoMove(transform.position + new Vector3(-10, 0, 0)));
	
					// Debug.Log("LEFT MOTION DETECTED");
					Debug.Log("UP MOTION DETECTED");
					last_blink_F = false;
					eye_state = c_states.IDLE;
					watch.Start();

				}
				else if(eye_state == c_states.RIGHT_MOVT)
				{
					idleTime.Stop();
					idleTime.Reset();
					mRigidBody.AddForce(Vector3.back * 400);
					// mRigidBody.AddForce(Vector3.right * 400);
					// if(moveCoroutine != null)
					// 	StopCoroutine(moveCoroutine);
					// moveCoroutine = StartCoroutine(DoMove(transform.position + new Vector3(10, 0, 0)));

					// Debug.Log("RIGHT MOTION DETECTED");
					Debug.Log("DOWN MOTION DETECTED");
					last_blink_F = false;
					eye_state = c_states.IDLE;
					watch.Start();
				}
				// Debug.Log("Passed");
        	}

			// if(Input.GetButtonDown("Fire1")) {
			// 	mRigidBody.AddForce(Vector3.left * 400);	
			// 	// mRigidBody.AddTorque(Vector3.left * 200);
			// }
			// if(Input.GetButtonDown("Fire2")) {
			// 	mRigidBody.AddForce(Vector3.right * 400);
			// 	// mRigidBody.AddTorque(Vector3.right * 200);
			// }
			// if(Input.GetButtonDown("Fire3")) {
			// 	mRigidBody.AddForce(Vector3.forward * 400);
			// 	// mRigidBody.AddTorque(Vector3.forward * 200);
			// }
			// if (Input.GetButton ("Horizontal")) {
			// 	// mRigidBody.AddForce(Vector3.back * Input.GetAxis("Horizontal") * 20);
			// 	mRigidBody.AddTorque(Vector3.back * Input.GetAxis("Horizontal")*200);
			// }
			// if (Input.GetButton ("Vertical")) {
			// 	// mRigidBody.AddForce(Vector3.right * 20);
			// 	mRigidBody.AddTorque(Vector3.right * Input.GetAxis("Vertical")*200);
			// }
			// if (Input.GetButtonDown("Jump")) {
			// 	if(mAudioSource != null && JumpSound != null){
			// 		mAudioSource.PlayOneShot(JumpSound);
			// 	}
			// 	mRigidBody.AddForce(Vector3.up*200);
			// }
		}
		if (ViewCamera != null) {
			Vector3 direction = (Vector3.up*2+Vector3.back)*2;
			RaycastHit hit;
			Debug.DrawLine(transform.position,transform.position+direction,Color.red);
			if(Physics.Linecast(transform.position,transform.position+direction,out hit)){
				ViewCamera.transform.position = hit.point;
			}else{
				ViewCamera.transform.position = transform.position+direction;
			}
			ViewCamera.transform.LookAt(transform.position);
		}
	}

	void OnCollisionEnter(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = true;
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.y > .5f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		} else {
			if (mAudioSource != null && HitSound != null && coll.relativeVelocity.magnitude > 2f) {
				mAudioSource.PlayOneShot (HitSound, coll.relativeVelocity.magnitude);
			}
		}
	}

	void OnCollisionExit(Collision coll){
		if (coll.gameObject.tag.Equals ("Floor")) {
			mFloorTouched = false;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals ("Coin")) {
			if(mAudioSource != null && CoinSound != null){
				mAudioSource.PlayOneShot(CoinSound);
			}
			Destroy(other.gameObject);
		}
	}

	public static void Read()
    {
		Debug.Log("In read function");
        while (_continue)
        {
            try
            {
                string message = _serialPort.ReadLine();
				int len = message.Split('\t').Length;
				// Debug.Log(len);
				if(len == 8)
				{
					int val = Int16.Parse(message.Split('\t')[2]);
					// int val = 500;
					// Debug.Log(val);
					// if(time_in_low.IsRunning && val > LOW_THRESHOLD)
					// {
					//     last_time_in_low = time_in_low.ElapsedMilliseconds;
					//     // Console.WriteLine("Time in LOW was: " + last_time_in_low + " ms");
					//     time_in_low.Stop();
					//     time_in_low.Reset();
					// }
					// if(time_in_high.IsRunning && val < HIGH_THRESHOLD)
					// {
					//     last_time_in_high = time_in_high.ElapsedMilliseconds;
					//     // Console.WriteLine("Time in LOW was: " + last_time_in_low + " ms");
					//     time_in_high.Stop();
					//     time_in_high.Reset();
					// }
					if(val > BLINK_THRESHOLD)
					{
						updateState(3);
					}
					else if(val > HIGH_THRESHOLD && eye_state != c_states.HIGH_THRES && eye_state != c_states.BLINK)
					{
						// time_in_high.Start();
						updateState(1);
					}
					else if(val < LOW_THRESHOLD && eye_state != c_states.LOW_THRES && eye_state != c_states.BLINK)
					{
						// time_in_low.Start();
						updateState(0);
					}
					else
					{
						if(idleTime.IsRunning == false)
						{
							idleTime.Start();
						}

						if(idleTime.ElapsedMilliseconds > 1000)
						{
							idleTime.Reset();
							idleTime.Stop();
							// if((eye_state == c_states.LOW_THRES && last_time_in_low < 50) || (eye_state == c_states.HIGH_THRES && last_time_in_high < 50))
							//     eye_state = c_states.BLINK;
							// else
							eye_state = c_states.IDLE;
						}
					}
				}
            }
            catch (TimeoutException) { }
        }
    }

	public static void updateState(int cross_thres)
    {
        switch (eye_state)
        {
            case c_states.IDLE:
            {
                if(cross_thres == 0)
                {
                    // Debug.Log("ID->LT;");
                    eye_state = c_states.LOW_THRES;
                }
                else if(cross_thres == 1)
                {
                    // Debug.Log("ID->HT;");
                    eye_state = c_states.HIGH_THRES;
                }
                else if(cross_thres == 3)
                {
                    // Debug.Log("ID->B;");
                    eye_state = c_states.BLINK;
                }
            }
                break;
            case c_states.LOW_THRES:
            {
                if(cross_thres == 1)
                {
                    // Debug.Log("LT->R;");
                    eye_state = c_states.RIGHT_MOVT;
                }
                else
                {
                    // Debug.Log("LT->ID;");
                    eye_state = c_states.IDLE;
                }
            }
                break;
            
            case c_states.HIGH_THRES:
            {
                if(cross_thres == 0)
                {
                    // Debug.Log("HT->L;");
                    eye_state = c_states.LEFT_MOVT;
                }
                else if(cross_thres == 3)
                {
                    // Debug.Log("HT->B;");
                    eye_state = c_states.BLINK;
                }
                else
                {
                    // Debug.Log("HT->ID;");
                    eye_state = c_states.IDLE;
                }
            }
                break;
            default:
                break;
        }
    }
}
