using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSupervisor : MonoBehaviour
{
	// public GameObject targetObject;
	private simpleMove capsuleMove;
	private GreyWolfOptimiser gwo;
	public programData ExperimentSettings;
	public bool DebuggingInformation;
	public GameObject swarmTarget;
	private int iterationNumber;
	// private batSearch bs;
	private particleSwarmOptimiser pso;
    // [SerializeField] private float communicationRange;
    // private GreyWolfOptimiser gwo;
    // Start is called before the first frame update

    void Start()
    {
        // ExperimentSettings.set_Target();
        // ExperimentSettings = this.GetComponent<programData>();
        // Debug.Log(ExperimentSettings.robotSpeed);

        capsuleMove = this.GetComponent<simpleMove>();
        gwo = this.GetComponent<GreyWolfOptimiser>();

        gwo.init_settings(ExperimentSettings, swarmTarget);
        capsuleMove.init_settings(ExperimentSettings);

        ExperimentSettings.set_TerrainLimits();
        iterationNumber = 0;
        capsuleMove.target = this.transform.position;
    }


    void Update()
    {
        if (capsuleMove.targetReached && (iterationNumber < ExperimentSettings.numberOfIterations))
        {
            gwo.point_fitnessCalculated = this.transform.position;
            gwo.get_NeighboursDict();
            gwo.set_myFitness(iterationNumber);
            bool isOmega = gwo.check_IsOmega();
            Vector3 nextPosition = gwo.calculate_NextPosition(isOmega, iterationNumber);
            // Vector3 nextPosition = gwo.calculate_RandomPosition();
            if (DebuggingInformation)
            {
                Debug.Log(string.Format("{0} - {1}", this.gameObject.name, nextPosition));
                Debug.Log(string.Format("{0} Iteration number {1}", this.gameObject.name, iterationNumber));
            }
            if (!ExperimentSettings.check_isOutOfBoundary(nextPosition) && !ExperimentSettings.check_isPointInObstacle(nextPosition))
            {
                capsuleMove.target = nextPosition;
                capsuleMove.targetReached = false;
                ++iterationNumber;
            }
        }
        else if (iterationNumber < ExperimentSettings.numberOfIterations)
        {
            // Debug.Log(capsuleMove.target);
            capsuleMove.moveTowardsTarget(DebuggingInformation, iterationNumber);
            capsuleMove.targetReached = capsuleMove.hasReachedTarget();
        }
    }

    //Particle Swarm Optimisation
    //void Start()
    //{
    //	// ExperimentSettings.set_Target();
    //	// ExperimentSettings = this.GetComponent<programData>();
    //	// Debug.Log(ExperimentSettings.robotSpeed);

    //	capsuleMove = this.GetComponent<simpleMove>();
    //	pso = this.GetComponent<particleSwarmOptimiser>();

    //	pso.init_settings(ExperimentSettings,swarmTarget);
    //	capsuleMove.init_settings(ExperimentSettings);

    //	ExperimentSettings.set_TerrainLimits();
    //	iterationNumber = 0;
    //	capsuleMove.target = this.transform.position;
    //}

    //void Update(){

    //	if(iterationNumber==49){
    //		Material m = this.GetComponent<Renderer>().material;
    //		m.SetColor("_Color", Color.red);	
    //	}

    //	if(capsuleMove.targetReached && (iterationNumber<ExperimentSettings.numberOfIterations)){
    //		pso.point_fitnessCalculated = this.transform.position;
    //		pso.get_NeighboursDict();
    //		pso.fitness = pso.calculate_fitnessOfPosition(iterationNumber,transform.position);
    //		Vector3 nextVelocity = pso.calculate_NextVelocity(iterationNumber);
    //		Vector3 nextPosition = pso.calculate_NextPosition(nextVelocity);
    //		// Vector3 nextPosition = gwo.calculate_RandomPosition();
    //		if(DebuggingInformation){
    //			Debug.Log(string.Format("{0} position generated is {1}, iteration: {2}", this.gameObject.name, nextPosition, iterationNumber));
    //		}		
    //		if(!ExperimentSettings.check_isOutOfBoundary(nextPosition) && !ExperimentSettings.check_isPointInObstacle(nextPosition)){
    //			pso.currentVelocity = nextVelocity;
    //			capsuleMove.target = nextPosition;
    //			if(DebuggingInformation){
    //				// Debug.Log(string.Format("BLah{0} - {1}", this.gameObject.name, nextVelocity.magnitude));
    //				Debug.Log(string.Format("{0} Iteration number {1}", this.gameObject.name, iterationNumber));
    //			}
    //			capsuleMove.targetReached = false;
    //			++iterationNumber;
    //		}
    //	}
    //	else if(iterationNumber<ExperimentSettings.numberOfIterations){
    //		// Debug.Log(capsuleMove.target);
    //		capsuleMove.moveTowardsTarget(DebuggingInformation,iterationNumber);
    //		capsuleMove.targetReached = capsuleMove.hasReachedTarget();
    //	}
    //}

    // //Bat Search Optimisation
    // void Start(){
    // 	capsuleMove = this.GetComponent<simpleMove>();
    // 	bs = this.GetComponent<batSearch>();

    // 	bs.init_settings(ExperimentSettings,swarmTarget);
    // 	capsuleMove.init_settings(ExperimentSettings);

    // 	ExperimentSettings.set_TerrainLimits();
    // 	iterationNumber = 0;
    // 	capsuleMove.target = this.transform.position;		
    // }

    // void Update(){
    // 	if(iterationNumber==199){
    // 		Material m = this.GetComponent<Renderer>().material;
    // 		m.SetColor("_Color", Color.red);
    // 	}

    // 	float avgLoudness = 0.0f;
    // 	Vector3 nextVelocity = new Vector3(0.0f,0.0f,0.0f);
    // 	Vector3 nextPosition = new Vector3(0.0f,0.0f,0.0f);
    // 	Vector3 x_best = new Vector3(0.0f,0.0f,0.0f);
    // 	bool flag = false;
    // 	if(capsuleMove.targetReached && (iterationNumber<ExperimentSettings.numberOfIterations)){
    // 		// Generate new solutions
    // 		bs.point_fitnessCalculated = this.transform.position;
    // 		bs.get_NeighboursDict(out avgLoudness);
    // 		bs.fitness = bs.calculate_fitnessOfPosition(iterationNumber,transform.position);
    // 		nextPosition = bs.calculate_NextPosition(out nextVelocity,out x_best);
    // 		if(Random.Range(0.0f,1.0f)>bs.pulseRate){
    // 			// bs.get_NeighboursDict(out avgLoudness);
    // 			// bs.fitness = bs.calculate_fitnessOfPosition(iterationNumber,transform.position);
    // 			// nextPosition = bs.calculate_NextPosition(out nextVelocity, out x_best);
    // 			x_best = bs.update_xBest(avgLoudness, x_best);
    // 			flag = true;
    // 		}
    // 		// Generate new solution
    // 		if(flag){
    // 			nextPosition = bs.update_NextPosition(out nextVelocity,x_best);
    // 		}

    // 		if(DebuggingInformation){
    // 			Debug.Log(string.Format("Before Checks value {0} position generated is {1}, iteration: {2}, Loudness : {3}", this.gameObject.name, nextPosition, iterationNumber, avgLoudness));
    // 		}

    // 		// pulse rate and loudness check
    // 		// if((Random.Range(0.0f,1.0f)<bs.loudness) && (bs.fitness< bs.calculate_fitnessOfPosition(iterationNumber,x_best)) && !ExperimentSettings.check_isOutOfBoundary(nextPosition) && !ExperimentSettings.check_isPointInObstacle(nextPosition)){
    // 		if((Random.Range(0.0f,1.0f)<bs.loudness) && !ExperimentSettings.check_isOutOfBoundary(nextPosition) && !ExperimentSettings.check_isPointInObstacle(nextPosition)){				
    // 			bs.currentVelocity = nextVelocity;
    // 			capsuleMove.target = nextPosition;
    // 			capsuleMove.targetReached = false;
    // 			++iterationNumber;
    // 			bs.update_pulseRateAndLoudness(iterationNumber);
    // 			if(DebuggingInformation){
    // 				Debug.Log(string.Format("Admissible value {0} position generated is {1}, iteration: {2}, Loudness : {3}", this.gameObject.name, nextPosition, iterationNumber, avgLoudness));
    // 			}
    // 		}
    // 	}
    // 	else if(iterationNumber<ExperimentSettings.numberOfIterations){
    // 		capsuleMove.moveTowardsTarget(DebuggingInformation,iterationNumber);
    // 		capsuleMove.targetReached = capsuleMove.hasReachedTarget();
    // 	}
    // }
}