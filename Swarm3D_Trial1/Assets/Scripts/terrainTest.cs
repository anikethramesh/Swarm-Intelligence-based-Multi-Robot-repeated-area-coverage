using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainTest : MonoBehaviour
{
	private int GridSize;
	private float TerrainSize, cellSize; //Assuming it is always going to be square.
	private Material m;
	// private Vector3 playerCoordinates;
	public GameObject player;
	// private Vector3 terrainDimensions;
	void Start()
	{
		m = this.GetComponent<Terrain>().materialTemplate;
		m.shader = Shader.Find("PDT Shaders/TestGrid");
		TerrainSize = this.GetComponent<Terrain>().terrainData.size.x;
		GridSize = m.GetInt("_GridSize");
		cellSize = TerrainSize/GridSize;
		// players = GameObject.FindGameObjectsWithTag("Player");
		// foreach(GameObject player in players)
		// 	cellToggler(player, 1);		
		player = GameObject.Find("Player1");
		// playerCoordinates = new Vector3(0,1,0);
		// TerrainSide = terrainDimensions
		// int toggle = 0;
		// m.SetInt("_GridSize", 20);
	}

	void cellToggler(GameObject player, int toggle){
		m.SetInt("_SelectedCellX", (int)(Mathf.Floor(player.transform.position.x/cellSize)+(GridSize/2)));
		m.SetInt("_SelectedCellY", (int)(Mathf.Floor(player.transform.position.z/cellSize)+(GridSize/2)));
		m.SetInt("_SelectCell",toggle);
	}

	// Update is called once per frame
	void Update()
	{	
		// m.SetInt("_SelectCell",0);
		// playerCoordinates.x = Mathf.Floor(player.transform.position.x/cellSize)+(GridSize/2);
		// playerCoordinates.z = Mathf.Floor(player.transform.position.z/cellSize)+(GridSize/2);
		// m.SetInt("_SelectedCellX", (int)playerCoordinates.x);
		// m.SetInt("_SelectedCellY", (int)playerCoordinates.z);
		// m.SetInt("_SelectCell",1);
		// players = GameObject.FindGameObjectsWithTag("Player");
		cellToggler(player, 1);
	}
}
