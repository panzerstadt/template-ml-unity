using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

// ref: https://github.com/Unity-Technologies/ml-agents/blob/0.5.0/docs/Learning-Environment-Create-New.md

public class RollerAgent : Agent {

	Rigidbody rBody;
	// Use this for initialization
	void Start () {
		rBody = GetComponent<Rigidbody> ();
		// when the script starts, get the component called Rigidbody
		// that is attached to this object.

	}

	// this is what should happen when the agent needs to reset,
	// either because it won or it fell (lost)
	public Transform Target;
	public override void AgentReset () {
		if (this.transform.position.y < -1.0) {
			// if the ball's y position is lower than -1
			// the agent fell
			this.transform.position = Vector3.zero;
			this.rBody.angularVelocity = Vector3.zero;
			this.rBody.velocity = Vector3.zero;
		} else {
			// this is the win condition
			// move target to a new spot
			Target.position = new Vector3 (
				Random.value * 8 - 4,
				0.5f,
				Random.value * 8 - 4);

		}
	}

	public override void CollectObservations () {
		// calculate relative position
		Vector3 relativePosition = Target.position - this.transform.position;

		// the following is all feature vectors, created line by line from observation

		// X Y and Z position (Y for height)
		AddVectorObs (relativePosition.x / 5);
		AddVectorObs (relativePosition.z / 5);
		//AddVectorObs (relativePosition.y / 5);

		// distance to edges (hardcoded.)
		AddVectorObs ((this.transform.position.x + 5) / 5);
		AddVectorObs ((this.transform.position.x - 5) / 5);
		AddVectorObs ((this.transform.position.z + 5) / 5);
		AddVectorObs ((this.transform.position.z - 5) / 5);
		//AddVectorObs ((this.transform.position.y + 5) / 5);
		//AddVectorObs ((this.transform.position.y - 5) / 5);

		// ask agent to learn velocity
		AddVectorObs (rBody.velocity.x / 5);
		AddVectorObs (rBody.velocity.z / 5);
		//AddVectorObs (rBody.velocity.y / 5);

		// All the values are divided by 5 to normalize the inputs to the
		// neural network to the range [-1,1]. (The number five is used
		// because the platform is 10 units across.)

		// we also need a continuous state space for this, to be set in the Brain prop

	}

	public float speed = 10;
	private float previousDistance = float.MaxValue;
	public override void AgentAction (float[] vectorAction, string textAction) {
		// setup rewards
		float distanceToTarget = Vector3.Distance (this.transform.position,
			Target.position);

		// if reach target, give big reward and set to done
		// 1.42f is probably the cube size from midpoint
		if (distanceToTarget < 1.42f) {
			AddReward (1.0f);
			Done ();
		}

		// time penalty
		AddReward (-0.05f);

		// fell off platform
		if (this.transform.position.y < -1.0) {
			AddReward (-1.0f);
			Done ();
		}

		// ~~~~ACTION SPACE~~~~

		// the agent has (n) number of types of actions it can take
		// since this agent is a ball, it is
		// x axis continuous movement (not discrete)
		// z axis continuous movement

		Vector3 controlSignal = Vector3.zero;
		controlSignal.x = vectorAction[0];
		controlSignal.z = vectorAction[1];
		// if we want the ball to move in the 3rd axis, apply y axis continuous movement
		rBody.AddForce (controlSignal * speed); // THE OUTPUT OF THE AGENT

		// to add the jump (explosino from my previous tutorial, add a discrete output)
		// that adds an explosion force to the object

		// the RL agent will learn on its own what actions are best when

		// in unity, under brain,
		// play actions map to actions here
		// The Index value corresponds to the index of the action array passed to
		// AgentAction() function. Value is assigned to action[Index] when Key is pressed.

	}

}