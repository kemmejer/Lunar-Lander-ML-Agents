using Unity.MLAgents;

public class ShipDecisionRequester : DecisionRequester
{
    public bool Enabled { get; set; }

    /// <summary>
    /// Returns whether the ship should make a decision
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool ShouldRequestDecision(DecisionRequestContext context)
    {
        return Enabled && base.ShouldRequestDecision(context);
    }

    /// <summary>
    /// Returns whether the ship should request an action
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override bool ShouldRequestAction(DecisionRequestContext context)
    {
        return Enabled && base.ShouldRequestAction(context);
    }
}
