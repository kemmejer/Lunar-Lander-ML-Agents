using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class ShipDecisionRequester : DecisionRequester
{
    public bool Enabled { get; set; }

    protected override bool ShouldRequestDecision(DecisionRequestContext context)
    {
        return Enabled && base.ShouldRequestDecision(context);
    }

    protected override bool ShouldRequestAction(DecisionRequestContext context)
    {
        return Enabled && base.ShouldRequestAction(context);
    }
}
