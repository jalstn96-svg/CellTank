using UnityEngine;

public static class RankManager 
{

    public static bool hasResult { get; private set; }
    public static int killCountResult { get; private set; }
    public static int maxAlertResult { get; private set; }

    public static int bestKillCount { get; private set; }
    public static int bestMaxAlert { get; private set; }
    
    public static void SetRsult(int killCount, int maxAlert)
    {
        hasResult = true;

        killCountResult = killCount;
        maxAlertResult = maxAlert;

        if (bestKillCount < killCountResult)
        {
            bestKillCount = killCountResult;
        }
        if(bestMaxAlert < maxAlertResult)
        {
            bestMaxAlert = maxAlertResult;
        }

    }
    

    

}
