using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Geometry_Dash.GameLogic
{
    public static class Level
    {
        public static int LevelLength { get; private set; }

        public static List<Obstacle> LoadLevel(int levelNumber, Form form)
        {
            // SRP: Метод LoadLevel тепер тільки координує процес, 
            // а не вирішує, як саме створювати кожен рівень.
            LevelBuilder builder = GetLevelBuilder(levelNumber);

            var obstacles = builder.Build(form);
            LevelLength = builder.Length;
            return obstacles;
        }

        // Окремий метод-фабрика, який відповідає тільки за вибір будівельника
        private static LevelBuilder GetLevelBuilder(int levelNumber) => levelNumber switch
        {
            1 => new Level1(),
            2 => new Level2(),
            3 => new Level3(),
            4 => new Level4(),
            _ => throw new System.Exception("Unknown level number")
        };
    }
}
