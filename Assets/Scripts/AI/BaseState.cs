public class BaseState
{
    protected CarAI carAI;
    protected AbilityController abilityController;

    public BaseState(CarAI carAI)
    {
        this.carAI = carAI;
        abilityController = carAI.GetComponent<AbilityController>();
    }

    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void Exit() { }
}
