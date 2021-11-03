using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class levelData
{
    // Handle level saving
    public static int level = 1; // Current level
    public static int savedLevel = 9;

    // Worker stat
    public static int receivingNum = 1; // by Default
    public static int pickingNum = 1; // by Default
    public static int packingNum = 1; // by Default
    public static int totalWorker = 3; // by Default

    // Public game statistics
    public static float capital = 80; // Capital given at the beginning a level
    public static float time; // Time of a level
    public static float vanExpense = 30; // Cost to run the van
    public static float vanSpeed = 7.0f; // Speed of the van
    public static float wage = 8; // Total wages for workers
    public static float workerSpd = 4.0f;
    public static float targetProfit; // Target profit after paying all the fees and costs
    public static int maximumOnVan = 8;
    public static int timeScale = 60; // Amount of time set to pay pay and utilities

    // Sound controller
    public static float audioSliderVal;

    // A static method to give level's properties 
    // based on level number
    public static void loadLevel()
    {
        // Load the game based on the chosen level
        time = 150 + 20 * (level - 1);
        targetProfit = 100 + (level - 1) * 50;
        // THIS TIME VALUE IS FOR TESTING ONLY
        //time = 500;

        // Handle no of workers
        loadWorkers();
        totalWorker = receivingNum + pickingNum + packingNum;
    }

    private static void loadWorkers()
    {
        switch (level)
        {
            case 4:
                receivingNum = 1;
                pickingNum = 1;
                packingNum = 2;
                break;
            case 5:
                receivingNum = 1;
                pickingNum = 2;
                packingNum = 2;
                break;
            case 6:
                receivingNum = 2;
                pickingNum = 2;
                packingNum = 2;
                break;
            case 7:
                receivingNum = 2;
                pickingNum = 2;
                packingNum = 3;
                break;
            case 8:
                receivingNum = 2;
                pickingNum = 3;
                packingNum = 3;
                break;
            case 9:
                receivingNum = 3;
                pickingNum = 3;
                packingNum = 3;
                break;
            default:
                receivingNum = 1;
                pickingNum = 1;
                packingNum = 1;
                break;
        }
    }

    public static void updateAudioVal(float value)
    {
        audioSliderVal = value;
    }
}
