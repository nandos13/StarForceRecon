using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAction : ActionAI
{
    public override float Evaluate(Agent a) { return Input.GetKey(KeyCode.P) ? 1 : 0;}
    public override void UpdateAction(Agent agent) { transform.localScale *= 1.01f;}
    public override void Enter(Agent agent){}
    public override void Exit(Agent agent){}
}
