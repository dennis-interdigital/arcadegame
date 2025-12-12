using System.Collections.Generic;
using UnityEngine;

public class CoroutineCache : MonoBehaviour
{
    const float WAIT_FOR_END_OF_FRAME = -1.0f;

    Dictionary<float, YieldInstruction> yieldInstructions;

    public void Init()
    {
        yieldInstructions = new Dictionary<float, YieldInstruction>();
    }

    public YieldInstruction WaitForSeconds(float val)
    {
        YieldInstruction result = null;
        if (yieldInstructions.ContainsKey(val))
        {
            result = yieldInstructions[val];
        }
        else
        {
            result = new WaitForSeconds(val);
            yieldInstructions[val] = result;
        }

        return result;
    }

    public YieldInstruction WaitForEndOfFrame()
    {
        YieldInstruction result = null;
        if (yieldInstructions.ContainsKey(WAIT_FOR_END_OF_FRAME))
        {
            result = yieldInstructions[WAIT_FOR_END_OF_FRAME];
        }
        else
        {
            result = new WaitForEndOfFrame();
            yieldInstructions[WAIT_FOR_END_OF_FRAME] = result;
        }

        return result;
    }
}
