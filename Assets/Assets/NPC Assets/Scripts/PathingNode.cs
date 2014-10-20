using System.Collections;

//TODO; CHANGE TO VECTOR3I
public class PathingNode {
	public Vector3i loc;
	public int g_score=0;
	private PathingNode _parent;

	public PathingNode(Vector3i _loc){
		this.loc = _loc;
	}

	public PathingNode(Vector3i _loc, PathingNode parent): this(_loc){
		_parent = parent;
		g_score = parent.g_score + 1;
	}

	public int GetFScore(Vector3i goal){
		return g_score + loc.DistanceSquared(goal);
	}

	public PathingNode GetParent(){
		return _parent;
	}

	public override bool Equals (object obj){
		PathingNode objNode = (PathingNode)obj;

		return obj != null && objNode != null && loc.Equals(objNode.loc);
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

}
