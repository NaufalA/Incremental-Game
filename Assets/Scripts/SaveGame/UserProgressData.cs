using System.Collections.Generic;

namespace SaveGame
{
    [System.Serializable]
    public class UserProgressData
    {
        public double gold;
        public List<int> resourceLevels = new List<int>();
    }
}