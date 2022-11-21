public class BaseState
{
    protected CarAI carAI;

    public BaseState(CarAI carAI)
    {
        this.carAI = carAI;
    }

    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void Exit() { }
}
