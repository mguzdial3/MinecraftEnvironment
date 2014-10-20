using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Environment;

public class NPCMovementController : MonoBehaviour {
	public static string SAVE_STRING = "Movement";

	private Vector3 _currGoal;
	private float _minDist= 1f;
	public float speed = 5f;
	private float maxDistance = 10f;

	private int maxChecksPerFrame = 100;

	//Path
	private Vector3[] path; 
	private int pathIndex;

	//States maybe
	private int currState = 0;
	private const int PATHING = 0;
	private const int ACTION = 1;

	private bool pause;

	//Appearance
	public NPCAppearanceHandler appearanceHandler;
	
	// Use this for initialization
	public void Init () {
		//Give it an initial random goal
		GenerateRandomGoal ();

		transform.position = _currGoal;

		Vector3i vec = Map.Instance.getEmptyLOC (new Vector3i(transform.position.x,transform.position.y,transform.position.z));
		transform.position = new Vector3 (vec.x, vec.y, vec.z);
		_currGoal = transform.position;
	}

	public void InitFromSave(Vector3 _location, Vector3[] _path){
		transform.position = _location;
		path = _path;

		if(path==null){
			_currGoal = transform.position;
		}
	}

	public void SetPause(bool _pause){
		pause = _pause;
	}

	// Update is called once per frame

	void Update () {
		if(!pause){
			Vector3 pauseVec = transform.position - Vector3.up;
			if (Map.Instance.IsPositionOpen (new Vector3i(pauseVec.x,pauseVec.y,pauseVec.z))) {
				transform.position-=Vector3.up*Time.deltaTime;
			}

			if(currState==PATHING){
					if (path == null) {
						path = AStar(transform.position,_currGoal);
						pathIndex = 0;

						if(path!=null){
							appearanceHandler.SetNewGoal(path[pathIndex]);
						}
					}
					else{
						//Are we done at this point
						Vector3 distToNext = path[pathIndex]-transform.position;
						distToNext+=Vector3.up;

						if(distToNext.magnitude<_minDist){
							pathIndex++;

							if(pathIndex>=path.Length){
								path=null;
							}
							else{
								appearanceHandler.SetNewGoal(path[pathIndex]);
							}
						}
						else{
							transform.position+=distToNext.normalized*Time.deltaTime*speed;
						}
					}

			}
		}
	}


	private Vector3[] AStar(Vector3 start, Vector3 goal){
		List<PathingNode> closedSet = new List<PathingNode> ();
		List<PathingNode> openSet = new List<PathingNode> ();

		PathingNode currNode = new PathingNode ((new Vector3i(transform.position.x,transform.position.y,transform.position.z)));
		openSet.Add (currNode);

		int count = 0;
	
		Vector3i goalI = new Vector3i (goal.x,goal.y,goal.z);
		while (openSet.Count!=0 && count<maxChecksPerFrame) {
			count++;
			currNode=openSet[0];

			foreach(PathingNode node in openSet){
				if(node.GetFScore(goalI)<currNode.GetFScore(goalI)){
					currNode = node;
				}
			}
			if(currNode.loc.DistanceSquared(goalI)<3 || count==maxChecksPerFrame){
				return GetPathFromNode(currNode);
			}
			else{
				openSet.Remove(currNode);
				closedSet.Add(currNode);

				List<Vector3i> moves = GetPossibleNextMovesFromPosition(currNode.loc);

				foreach(Vector3i move in moves){
					PathingNode potentialNode = new PathingNode(move);

					if(closedSet.Contains(potentialNode)){
					}
					else if(openSet.Contains(potentialNode)){
						int index = openSet.IndexOf(potentialNode);
						if(openSet[index].g_score > potentialNode.g_score){
							openSet[index]=potentialNode;
						}
					}
					else{
						openSet.Add(potentialNode);
					}
				}
			}

		}
		return null;
	}

	public void SetCurrGoal(Vector3 goal){
		path = null;
		_currGoal = goal;
	}

	private Vector3[] GetPathFromNode(PathingNode goal){
		List<Vector3> protoPath = new List<Vector3>();

		if (goal.GetParent () == null) {
			protoPath.Add (new Vector3(goal.loc.x,goal.loc.y,goal.loc.z));
			goal = goal.GetParent();	
		}
		else{
			while (goal.GetParent()!=null) {
				protoPath.Add (new Vector3(goal.loc.x,goal.loc.y,goal.loc.z));
				goal = goal.GetParent();
			}
		}

		protoPath.Reverse ();

		return protoPath.ToArray ();

	}

	private List<Vector3i> GetPossibleNextMovesFromPosition(Vector3i position){
		List<Vector3i> nextMoves = new List<Vector3i>();

		//Left
		nextMoves.Add ( position + Vector3i.left);

		//Right
		nextMoves.Add ( position + Vector3i.right);

		//Forward
		nextMoves.Add ( position + Vector3i.forward);

		//Back
		nextMoves.Add ( position + Vector3i.back);

		//Go through and check to make sure all are acceptable
		Map _map = Map.Instance;

		for(int i = 0; i<nextMoves.Count; i++){
			if(_map.IsPositionOpen(nextMoves[i]) && !_map.IsPositionOpen(nextMoves[i]+Vector3i.down)){ //Place to stand
				//Good to go
			}
			else if(_map.IsPositionOpen(nextMoves[i]) && _map.IsPositionOpen(nextMoves[i]+Vector3i.down) && !_map.IsPositionOpen(nextMoves[i]+new Vector3i(0,-2,0))){
				//Jump down one
				nextMoves[i]+=Vector3i.down;
			}
			else if(!_map.IsPositionOpen(nextMoves[i]) && _map.IsPositionOpen(nextMoves[i]+Vector3i.up)){
				//Jump up one
				nextMoves[i]+=Vector3i.up;
			}
			else{
				nextMoves[i]=position; //Way to get rid of it
			}

		}

		return nextMoves;
	}

	private void GenerateRandomGoal(){
		_currGoal = transform.position + Vector3.right * (Random.Range (-1f, 1f) * maxDistance)
			+ Vector3.forward * (Random.Range (-1f, 1f) * maxDistance);
		_currGoal.x = (int)_currGoal.x;
		_currGoal.y = (int)_currGoal.y;
		_currGoal.z = (int)_currGoal.z;
	}

	public string GetSaveString(bool hasControl, Vector3 playerPos=default(Vector3)){
		string saveString = SAVE_STRING + " ";
		if(!hasControl){
			saveString += transform.position.x+" "+transform.position.y+" "+transform.position.z+" ";

			if(path!=null){
				for(int i = pathIndex; i<path.Length; i++){
					saveString+=path[pathIndex].x+" "+path[pathIndex].y+" "+path[pathIndex].z+" ";
				}
			}
		}
		else{
			saveString += playerPos.x+" "+playerPos.y+" "+playerPos.z+" ";
		}

		return saveString;
	}

	public Vector3 GetGoal(){
		return _currGoal;
	}
}
