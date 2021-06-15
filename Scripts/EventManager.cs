using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    EnemyController AIController;
    
    void Start()
    {
        AIController = GameObject.Find("EnemyAIController").GetComponent<EnemyController>();
    }

    void Update()
    {
        
    }

    public void LevelHasChanged()
    {
        if (AIController.bActive)
        {
            AIController.ReevaluateAI();
        }
    }
}
