using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public FloorZone floor1;
    public FloorZone floor2;
    public FloorZone floor3;

    public DoorController doorController;

    private bool isCleared = false;
    private bool gameStarted = false;

    public Action OnStageCleared;

    public void StartMiniGame()
    {
        gameStarted = true;
        CheckStageClear();
    }

    public void CheckStageClear()
    {
        if (!gameStarted)
        {
            isCleared = false;

            if (doorController != null)
                doorController.SetDoorState(false);

            return;
        }

        bool floor1Clear = floor1 != null && floor1.IsSolved();
        bool floor2Clear = floor2 != null && floor2.IsSolved();
        bool floor3Clear = floor3 != null && floor3.IsSolved();

        bool allClear = floor1Clear && floor2Clear && floor3Clear;

        if (allClear == isCleared)
            return;

        isCleared = allClear;

        if (doorController != null)
            doorController.SetDoorState(isCleared);

        if (isCleared)
        {
            OnStageCleared?.Invoke();
        }
    }
}