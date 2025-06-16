using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPointCommand : ICommand
{
    public void Execute()
    {
        PointManager.Instance.ResetStats();
    }
}
