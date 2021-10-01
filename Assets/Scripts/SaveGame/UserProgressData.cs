using System.Collections.Generic;

namespace SaveGame
{
    [System.Serializable]
    public class UserProgressData
    {
        public double goldTotal;
        public double goldSpent;
        public List<int> resourceLevels = new List<int>();
    }
}