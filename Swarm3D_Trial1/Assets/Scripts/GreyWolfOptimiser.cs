using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyWolfOptimiser : MonoBehaviour
{

	// private int numberOfRobots;
	private int numberOfIterations;
	private float explorationFactor;
	private GameObject targetPoint;
	private float communicationRange;
	private float x_uppLim, x_lowLim, z_uppLim, z_lowLim;
	private List<KeyValuePair<Vector3, float>> neighbourInformation;
	private Vector3 point_Start;
	public Vector3 point_fitnessCalculated; //needs to be public
	public float fitness; //needs to be public
	private bool isOmega;
	// private MovementSupervisor robotMover;
	private int j;

	public void init_settings(programData experimentSettings, GameObject swarmTarget){
		point_Start = this.transform.position;
		neighbourInformation = new List<KeyValuePair<Vector3, float>>();
		fitness = 0.0f;
		// coverageTotal = 0.0f;
		targetPoint = swarmTarget;
		explorationFactor = experimentSettings.explorationFactor;
		numberOfIterations = experimentSettings.numberOfIterations;
		communicationRange = experimentSettings.communicationRange;
		x_lowLim = experimentSettings.x_lowLim;
		x_uppLim = experimentSettings.x_uppLim;
		z_lowLim = experimentSettings.z_lowLim;
		z_uppLim = experimentSettings.z_uppLim;
	}


	public void get_NeighboursDict(){
		// neighbours.Clear();
		neighbourInformation.Clear();
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, communicationRange);
		for(int i= 0;i<hitColliders.Length;i++){
			if((hitColliders[i].gameObject.tag == "Player")&&(hitColliders[i].gameObject.transform != transform)){
				// neighbour.Add(hitColliders[i].gameObject);
				GreyWolfOptimiser gwo = hitColliders[i].gameObject.GetComponent<GreyWolfOptimiser>();
				neighbourInformation.Add(new KeyValuePair<Vector3, float>(gwo.point_fitnessCalculated,gwo.fitness));
			}
		}
	}

	// Call get_NeighboursDict() before calling this!!!!!!!!!!!!
	public void set_myFitness(int currentIteration){
		float k_intraSwarmDistance = 0.0f, k_coverageIndividual = 0.0f, k_distanceFromGoal = 0.0f;
		float intraSwarmDistance = 0.0f, coverageIndividual = 0.0f, distanceFromGoal = 0.0f;
		fitness = 0.0f;
		k_coverageIndividual = 1 - (currentIteration/numberOfIterations);
		k_distanceFromGoal = 1 + (currentIteration/numberOfIterations);
		if(neighbourInformation.Count > 0){
			foreach(KeyValuePair<Vector3, float> neighbour in neighbourInformation){
				intraSwarmDistance += communicationRange/((this.transform.position - neighbour.Key).magnitude);
			}
			k_intraSwarmDistance = 1 - (currentIteration/numberOfIterations);
			fitness+= k_intraSwarmDistance*intraSwarmDistance;		
		}
		distanceFromGoal = (transform.position - targetPoint.transform.position).magnitude;
		coverageIndividual = (point_Start - transform.position).magnitude;
		// coverageIndividual = coverageTotal +	
		fitness+= (k_coverageIndividual*coverageIndividual) - (k_distanceFromGoal*distanceFromGoal);
		// fitness+= (0*coverageIndividual) + (k_distanceFromGoal*distanceFromGoal);
	}

	// Call get_NeighboursDict() before calling this!!!!!!!!!!!!
	public bool check_IsOmega(){
		bool isOmega = false;
		Material m = this.GetComponent<Renderer>().material;
		m.SetColor("_Color", Color.white);
		if(neighbourInformation.Count > 2){
    		// neighbourInformation.Sort((p1,p2)=>p1.Value.CompareTo(p2.Value));
    		neighbourInformation.Sort((x, y) => y.Value.CompareTo(x.Value));
    		// Debug.Log(string.Format("{2} - Zero object Fitness {0} 7th Object Fitness {1}", neighbourInformation[0].Value,neighbourInformation[6].Value,this.gameObject.name));
    		//Check if the my fitness is more than the first 3 fittest solutions in the neighbours
    		if(neighbourInformation[2].Value>fitness){
    			isOmega = true;
				m.SetColor("_Color", Color.red);
    		}
		}
		return isOmega;
	}

	void calculate_Constants(int currentIteration, out Vector3 a, out Vector3 r1, out Vector3 r2){
		a = explorationFactor*(1 - ((float)currentIteration/(float)numberOfIterations))*(new Vector3(1.0f,1.0f,1.0f));
		r1 = new Vector3(Random.Range(0f, 1.0f),Random.Range(0f, 1.0f),Random.Range(0f, 1.0f));
		r2 = new Vector3(Random.Range(0f, 1.0f),Random.Range(0f, 1.0f),Random.Range(0f, 1.0f));
		// Debug.Log(string.Format("{0},{1}",this.gameObject.name,a));
	}

	public Vector3 calculate_NextPosition(bool isOmega,int currentIteration){
		Vector3 nextPosition = new Vector3(0.0f,0.0f,0.0f);
		Vector3 a = new Vector3(0.0f,0.0f,0.0f);
		Vector3 r1 = new Vector3(0.0f,0.0f,0.0f);
		Vector3 r2 = new Vector3(0.0f,0.0f,0.0f);
		Vector3 A = new Vector3(0.0f,0.0f,0.0f);
		Vector3 C = new Vector3(0.0f,0.0f,0.0f);
		Vector3 D = new Vector3(0.0f,0.0f,0.0f);
		if (isOmega){
			for(int i=0;i<3;i++){
				calculate_Constants(currentIteration,out a, out r1, out r2);
				A = (2*Vector3.Scale(a,r1)) - a;
				C = 2*r2;
				D = Vector3.Scale(C,neighbourInformation[i].Key) - transform.position;
				nextPosition +=(neighbourInformation[i].Key - Vector3.Scale(A,D))/3;
			}
		}
		else{
			calculate_Constants(currentIteration,out a, out r1, out r2);
			A = (2*Vector3.Scale(a,r1)) - a;
			C = 2*r2;
			D = Vector3.Scale(C, targetPoint.transform.position) - transform.position;
			nextPosition = (targetPoint.transform.position - Vector3.Scale(A,D));
		}
		nextPosition.y = 1; //Because the motion is in XZ plane.
		return nextPosition;
	}

	public Vector3 calculate_RandomPosition(){
		Vector3 nextPosition = new Vector3(Random.Range(x_lowLim, x_uppLim),1,Random.Range(z_lowLim, z_uppLim));
		// Debug.Log(string.Format("{0},{1},{2},{3}",x_lowLim, x_uppLim,z_lowLim, z_uppLim));
		return nextPosition;
	}
}
