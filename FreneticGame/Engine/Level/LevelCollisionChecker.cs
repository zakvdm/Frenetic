using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public class LevelCollisionChecker
    {
        //private int _numberOfRows, _numberOfColumns = 0;
        private List<int>[] _collisionArray;

        public int NumberOfRows
        {
            get
            {
                if (_collisionArray == null)
                    return 0; 
                if (_collisionArray.Length == 0)
                    return 0;
                return _collisionArray[0].Count;
            }
        }

        public int NumberOfColumns
        {
            get
            {
                if (_collisionArray == null)
                    return 0;
                return (_collisionArray.Length);
            }
        }

        public List<int>[] CollisionArray
        {
            get
            {
                return _collisionArray;
            }
        }

        public LevelCollisionChecker(List<int>[] collisionArray)
        {   
            _collisionArray = collisionArray;
        }

        public bool DoesBoundingBoxCollideWithLevel(GameplayObject testingObject)
        {
            if ((testingObject.Position.X < 0) || ((testingObject.Position.X + testingObject.Width) >= NumberOfColumns)
                || (testingObject.Position.Y < 0) || ((testingObject.Position.Y + testingObject.Height) >= NumberOfRows))
                return true;
            for (int col = (int)testingObject.Position.X; col < (int)testingObject.Position.X + testingObject.Width; col++)
            {
                for (int row = (int)testingObject.Position.Y; row < (int)testingObject.Position.Y + testingObject.Height; row++)
                {
                   
                    if (CollisionArray[col][row] == 1)
                        return true;
                }
            }
            return false;
        }
    }
}
