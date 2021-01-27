using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleSwarmOptimiser : MonoBehaviour
{
	private float communicationRange;
	public Vector3 currentVelocity;
	private List<KeyValuePair<Vector3, float>> neighbourInformation;
	private List<Vector3> particlePositions;
	public Vector3 point_fitnessCalculated; //needs to be public
	public float fitness; //needs to be public
	private GameObject targetPoint;
	private int numberOfIterations;
	private Vector3 point_Start;
	// private KeyValuePair<Vector3, float> localBest_kvp, globalBest_kvp;
	public struct pso_best{
		public Vector3 pos;
		public float fitness;

	    public pso_best(Vector3 particlePos, float particle_fitness){
	        pos = particlePos;
	        fitness = particle_fitness;
	    }
	}
	private pso_best localBest_kvp, globalBest_kvp;

	public void init_settings(programData experimentSettings, GameObject swarmTarget){
		currentVelocity = new Vector3(0.0f,0.0f,0.0f);
		neighbourInformation = new List<KeyValuePair<Vector3, float>>();
		particlePositions = new List<Vector3>();
		fitness = 0.0f;
		targetPoint = swarmTarget;
		numberOfIterations = experimentSettings.numberOfIterations;
		communicationRange = experimentSettings.communicationRange;	
		point_Start = this.transform.position;
		localBest_kvp = new pso_best(this.transform.position,calculate_fitnessOfPosition(0, this.transform.position));
		globalBest_kvp = new pso_best(calculate_globalBest(),calculate_fitnessOfPosition(0, targetPoint.transform.position));
	}

	private void calculate_Constants(int currentIteration, out float w,out float r1, out float r2, out float c1, out float c2){
		w = 6.0f - (5.0f*(float)currentIteration/(float)numberOfIterations);
		// 	w = 3.0f*(2 - ((float)currentIteration/(float)numberOfIterations));
		// w = Random.Range(0.0f,6.0f);
		r1 = Random.Range(0.0f,1.0f);
		r2 = Random.Range(0.0f,1.0f);
		c1 = 2.0f;
		c2 = 2.0f;
	}

	public float calculate_fitnessOfPosition(int currentIteration, Vector3 particlePosition){
		float k_intraSwarmDistance = 0.0f, k_coverageIndividual = 0.0f, k_distanceFromGoal = 0.0f;
		float intraSwarmDistance = 0.0f, coverageIndividual = 0.0f, distanceFromGoal = 0.0f;
		float particle_fitness = 0.0f;
		k_coverageIndividual = 1 - (currentIteration/numberOfIterations);
		k_distanceFromGoal = 1 + (currentIteration/numberOfIterations);
		if(neighbourInformation.Count > 0){
			foreach(KeyValuePair<Vector3, float> neighbour in neighbourInformation){
				intraSwarmDistance += communicationRange/((particlePosition - neighbour.Key).magnitude);
			}
			k_intraSwarmDistance = 1 - (currentIteration/numberOfIterations);
			particle_fitness+= k_intraSwarmDistance*intraSwarmDistance;		
		}
		distanceFromGoal = (particlePosition - targetPoint.transform.position).magnitude;
		coverageIndividual = (point_Start - particlePosition).magnitude;
		particle_fitness+= (k_coverageIndividual*coverageIndividual) - (k_distanceFromGoal*distanceFromGoal);
		// particle_fitness= - (k_distanceFromGoal*distanceFromGoal);
		return particle_fitness;
	}

	public void get_NeighboursDict(){
		// neighbours.Clear();
		neighbourInformation.Clear();
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, communicationRange);
		for(int i= 0;i<hitColliders.Length;i++){
			if((hitColliders[i].gameObject.tag == "Player")&&(hitColliders[i].gameObject.transform != transform)){
				// neighbour.Add(hitColliders[i].gameObject);
				particleSwarmOptimiser pso = hitColliders[i].gameObject.GetComponent<particleSwarmOptimiser>();
				neighbourInformation.Add(new KeyValuePair<Vector3, float>(pso.point_fitnessCalculated,pso.fitness));
			}
		}
	}


	// Call get_NeighboursDict() before calling this!!!!!!!!!!!!
	// the particlePositions should be populated for this to actually work
	private Vector3 calculate_LocalBest(int currentIteration){
		//Parse through list of previous positions and get fittest
		Vector3 localBest = new Vector3(0.0f,0.0f,0.0f);
		if(particlePositions.Count>1){
			float local_fitness = Mathf.NegativeInfinity;
			float buff = 0.0f;
			foreach(Vector3 particle_pos in particlePositions){
				buff = calculate_fitnessOfPosition(currentIteration, particle_pos);
				if(buff>local_fitness){
					local_fitness = buff;
					localBest = particle_pos;
				}
			}
		}
		return localBest;
	}

	private Vector3 calculate_globalBest(){
		Vector3 globalBest = targetPoint.transform.position;
		if(neighbourInformation.Count>0){
			float local_fitness = Mathf.NegativeInfinity;
			foreach(KeyValuePair<Vector3, float> neighbour in neighbourInformation){
				if(neighbour.Value>local_fitness){
					local_fitness = neighbour.Value;
					globalBest = neighbour.Key;
				}	
			}
		}
		return globalBest;
	}

	public Vector3 calculate_NextVelocity(int currentIteration){
		Vector3 nextVelocity = new Vector3(0.0f,0.0f,0.0f);
		float w, r1,r2, c1,c2;
		Vector3 bestPosition_particle = new Vector3(0.0f,0.0f,0.0f);
		Vector3 bestPosition_global = new Vector3(0.0f,0.0f,0.0f);
		calculate_Constants(currentIteration,out w,out r1, out r2, out c1, out c2);
		// float buff1,buff2;
		bestPosition_particle = calculate_LocalBest(currentIteration);
		bestPosition_global = calculate_globalBest();

		if(calculate_fitnessOfPosition(currentIteration,bestPosition_particle)> localBest_kvp.fitness){
			localBest_kvp.pos = bestPosition_particle;
			localBest_kvp.fitness = calculate_fitnessOfPosition(currentIteration,bestPosition_particle);
		}
		else{
			bestPosition_particle = localBest_kvp.pos;
		}

		if(calculate_fitnessOfPosition(currentIteration,bestPosition_global)> globalBest_kvp.fitness){
			globalBest_kvp.pos = bestPosition_global;
			globalBest_kvp.fitness = calculate_fitnessOfPosition(currentIteration,bestPosition_global);
		}
		else{
			bestPosition_global = globalBest_kvp.pos;
		}

		nextVelocity  = (w*currentVelocity);
		nextVelocity += (c1*r1*(bestPosition_particle - transform.position));
		nextVelocity += (c2*r2*(bestPosition_global - transform.position));
		nextVelocity = Vector3.ClampMagnitude(nextVelocity,30);
		// Debug.Log(string.Format("{0} - {1}", this.gameObject.name, nextPosition));
		return nextVelocity;
	}

	public Vector3 calculate_NextPosition(Vector3 nextVelocity){
		float stepSize = 1.0f;
		// float tolerance = 0.0f;
		Vector3 nextPosition = transform.position + (stepSize*nextVelocity);
		// nextPosition = Vector3.ClampMagnitude(nextPosition,97);
		nextPosition.y = 1; //Because the motion is in XZ plane.
		return nextPosition;
	}
}
