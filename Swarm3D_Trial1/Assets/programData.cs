using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "programData", menuName = "ScriptableObjects/programDataFile", order = 1)]
public class programData : ScriptableObject
{
	// Start is called before the first frame update
	public int rayCastRange;
	public float robotSpeed;
	// public GameObject swarmTarget;
	public string LogFileDirectory;
	public float targetReachtolerance;
	public int numberOfIterations;
	public float communicationRange;
	public float x_uppLim,x_lowLim,z_uppLim,z_lowLim;
	public int explorationFactor;
    
	public void set_TerrainLimits(){
		int offset = 2;
		GameObject go = GameObject.Find("Terrain");
		Terrain terr = go.GetComponent<Terrain>();
		x_lowLim = -terr.terrainData.size.x/2 + offset;
		x_uppLim = -x_lowLim;
		z_lowLim = -terr.terrainData.size.z/2 + offset;
		z_uppLim = -z_lowLim;
		// Debug.Log(string.Format("Boundaries: {0},{1},{2},{3}",x_lowLim,x_uppLim,z_lowLim,z_uppLim));
	}

	public bool check_isOutOfBoundary(Vector3 robotPosition){
		bool isOutOfBoundary = false;
		if(robotPosition.x>= x_uppLim || robotPosition.x <= x_lowLim || robotPosition.z<=z_lowLim || robotPosition.z >= z_uppLim)
			isOutOfBoundary = true;
		return isOutOfBoundary;
	}

	// public void set_Target(){
	// 	swarmTarget = GameObject.Find("SwarmTarget");
	// }

	public bool check_isPointInObstacle(Vector3 givenPosition){
		Collider[] hitColliders = Physics.OverlapSphere(givenPosition, 1.0f);
		bool spawnedInObstacle = false;
		int i = 0;
		while (i < hitColliders.Length)
		{
			if(hitColliders[i].gameObject.tag == "Obstacle")
				spawnedInObstacle|= true;
			i++;
		}
		return spawnedInObstacle;
	}

}
