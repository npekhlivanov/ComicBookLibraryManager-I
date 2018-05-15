using System;

namespace StreamsTest
{
    class GameResult
    {
        // GameDate,TeamName,HomeOrAway,Goals,GoalAttempts,ShotsOnGoal,ShotsOffGoal,PosessionPercent
        public DateTime GameDate { get; set; }
        public string TeamName { get; set; }
        public HomeOrAway HomeOrAway { get; set; }
        public int Goals { get; set; }
        public int GoalAttempts { get; set; }
        public int ShotsOnGoal { get; set; }
        public int ShotsOffGoal { get; set; }
        public double PosessionPercent { get; set; }
    }

    public enum HomeOrAway //: byte -> the underlying data type may be modified, int is default
    {
        Home,
        Away
    }
}
