using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using UnityEditor;


public class simpleMove : MonoBehaviour
{	
	//Range settings
	private int range;
	private float speed;
	// private bool isThereAnything = false;
	private Vector3 endPoint;
	private bool[] rayHitStatus;
	private RaycastHit[] hitArray;
	private int left_30deg, front_0Deg, right_30deg, left_60deg, right_60deg;
	private float targetReachtolerance;
	private string filePath;
	//Public variables
	public Vector3 target;
	public bool targetReached;

	public void init_settings(programData experimentSettings){
		range = experimentSettings.rayCastRange;
		speed = experimentSettings.robotSpeed;
		targetReachtolerance = experimentSettings.targetReachtolerance;
		rayHitStatus = new bool[5];
		hitArray = new RaycastHit[5];
		left_30deg = 0;
		front_0Deg = 1;
		right_30deg = 2;
		left_60deg = 3;
		right_60deg = 4;
		filePath = experimentSettings.LogFileDirectory+(this.name)+".txt";
		targetReached = true;
	}

	void writePositionToFile(string fileName, int programIterationNumber){
		using(StreamWriter writeText = File.AppendText(fileName)){
			writeText.WriteLine(transform.position.x+","+transform.position.y+","+transform.position.z+","+Time.time+","+programIterationNumber);
			writeText.Close();
		}
	}


	void drawRays(){
		Vector3 endPoint = transform.forward*range;
		// endPoint = Quaternion.AngleAxis(45, Vector3.up)*endPoint;
		Debug.DrawRay(transform.position, endPoint, Color.red);
		Debug.DrawRay(transform.position, Quaternion.AngleAxis(30, Vector3.up)*endPoint, Color.red);
		Debug.DrawRay(transform.position, Quaternion.AngleAxis(-30, Vector3.up)*endPoint, Color.red);
		Debug.DrawRay(transform.position, Quaternion.AngleAxis(60, Vector3.up)*endPoint, Color.red);
		Debug.DrawRay(transform.position, Quaternion.AngleAxis(-60, Vector3.up)*endPoint, Color.red);
	}

	void emitRays(){
		Vector3 endPoint = transform.forward;
		// RaycastHit hit_front, hit_left, hit_right;
		// RaycastHit hit;
		// int left=0, front = 1, right = 2;
		rayHitStatus[right_30deg] = Physics.Raycast(transform.position, Quaternion.AngleAxis(30, Vector3.up)*endPoint, out hitArray[right_30deg], range);
		rayHitStatus[left_30deg] = Physics.Raycast(transform.position, Quaternion.AngleAxis(-30, Vector3.up)*endPoint, out hitArray[left_30deg], range);
		rayHitStatus[front_0Deg] = Physics.Raycast(transform.position, endPoint, out hitArray[front_0Deg], range);
		rayHitStatus[left_60deg] = Physics.Raycast(transform.position, Quaternion.AngleAxis(-60, Vector3.up)*endPoint, out hitArray[left_60deg], range);
		rayHitStatus[right_60deg] = Physics.Raycast(transform.position, Quaternion.AngleAxis(60, Vector3.up)*endPoint, out hitArray[right_60deg], range);
		// Debug.Log(string.Format("RayStatus: {0},{1},{2}",rayHitStatus[left_30deg],rayHitStatus[front_0Deg],rayHitStatus[right_30deg]));
		// return (rayHitStatus[left_30deg]||rayHitStatus[front_0Deg]||rayHitStatus[right_30deg]||rayHitStatus[left_60deg]||rayHitStatus[right_60deg]);
	}

	public bool hasReachedTarget(){
		bool flag = false;
		emitRays();
		// Debug.Log((transform.position - target).magnitude);
		if(rayHitStatus[front_0Deg]){
			if((hitArray[front_0Deg].collider.gameObject.CompareTag("Player") || hitArray[front_0Deg].collider.gameObject.CompareTag("Player")) && (transform.position - target).magnitude < (range+ 0.5))
				flag = true;
			else
				flag = false;
		}
		else{
			if((transform.position - target).magnitude>targetReachtolerance)
				flag = false;
			else
				flag = 	true;
		}
		return flag;
	}

	public void moveTowardsTarget(bool DebugInfo,int programIterationNumber){
		// if(DebugInfo)
		// 	Debug.Log(target);
		Vector3 dir = Vector3.Normalize(target - transform.position);
		if(rayHitStatus[front_0Deg]){
			if(hitArray[front_0Deg].collider.gameObject.CompareTag("Obstacle"))
				dir+=hitArray[front_0Deg].normal*120;
			else if(hitArray[front_0Deg].collider.gameObject.CompareTag("Player"))
				dir+=hitArray[front_0Deg].normal*120;
		}
		if(rayHitStatus[left_30deg]){
			if(hitArray[left_30deg].collider.gameObject.CompareTag("Obstacle"))
				dir+=hitArray[left_30deg].normal*100;
			else if(hitArray[left_30deg].collider.gameObject.CompareTag("Player"))
				dir+=hitArray[left_30deg].normal*100;
		}
		if(rayHitStatus[right_30deg]){
			if(hitArray[right_30deg].collider.gameObject.CompareTag("Obstacle"))
				dir+=hitArray[right_30deg].normal*100;
			else if(hitArray[right_30deg].collider.gameObject.CompareTag("Player"))
				dir+=hitArray[right_30deg].normal*100;
		}
		if(rayHitStatus[right_60deg]){
			if(hitArray[right_60deg].collider.gameObject.CompareTag("Obstacle"))
				dir+=hitArray[right_60deg].normal*80;
			else if(hitArray[right_60deg].collider.gameObject.CompareTag("Player"))
				dir+=hitArray[right_60deg].normal*80;
		}
		if(rayHitStatus[left_60deg]){
			if(hitArray[left_60deg].collider.gameObject.CompareTag("Obstacle"))
				dir+=hitArray[left_60deg].normal*80;
			else if(hitArray[left_60deg].collider.gameObject.CompareTag("Player"))
				dir+=hitArray[left_60deg].normal*80;
		}

		Quaternion rot = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, 0.5f);
		transform.position += transform.forward*speed*Time.deltaTime;
		drawRays();
		writePositionToFile(filePath,programIterationNumber);
	}
}
