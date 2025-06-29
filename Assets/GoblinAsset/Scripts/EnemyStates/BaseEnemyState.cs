using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyState
{
    protected GoblinAI goblinAI;
    protected NavMeshAgent navMeshAgent;
    protected GameObject player;
    protected float updateTimeStep;

    public BaseEnemyState(GoblinAI goblinAI, NavMeshAgent navMeshAgent, GameObject player) {
        this.goblinAI = goblinAI;
        this.navMeshAgent = navMeshAgent;
        this.player = player;
        this.updateTimeStep = goblinAI.GetUpdateTimeStep();
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}