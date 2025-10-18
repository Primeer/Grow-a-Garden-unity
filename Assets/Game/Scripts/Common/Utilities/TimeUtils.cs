using System;
using GamePush;

namespace Game.Scripts.Common.Utilities
{
    public static class TimeUtils
    {
        public static long CurrentTimeTicks() => GP_Server.Time().Ticks;
        
        public static float CalculateProgressTime(float savedProgressTime, long saveTimeTicks)
        {
            return savedProgressTime + GetDeltaSeconds(saveTimeTicks);
        }
        
        public static float GetDeltaSeconds(long saveTimeTicks)
        {
            var dateTime = new DateTime(saveTimeTicks);
            return (float)(GP_Server.Time() - dateTime).TotalSeconds;
        }
    }
}