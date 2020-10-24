using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionHandler : MonoBehaviour
{
    
    public ScriptableHealth playerStatus;
    public ProgressionHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void DiscoverDash()
    {
        playerStatus.canDash = true;
    }

}
