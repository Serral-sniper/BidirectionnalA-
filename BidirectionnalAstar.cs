using SuperLinq.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding_Astar
{
    class BidirectionnalAstar
    {
        private Astar Forward;
        private Astar Backward;
        public static Dictionary<(int, int), Node> MergedGrids;
        //public static (ulong, ulong) Counter;
        public static char[,] mapModel;

        public BidirectionnalAstar(int walls)
        {
            mapModel = MapModel.mapGeneration(walls);
            MergedGrids = new Dictionary<(int, int), Node>();
            //Counter = (0, 0);
            Path();
        }
        public void Path()
        {
            Task task1 = Task.Run(() => 
            {
                Forward = new Astar(MapModel._Departure, MapModel._End);
                return Task.CompletedTask;
            });
            Task task2 = Task.Run(() =>
            {
                Backward = new Astar(MapModel._End, MapModel._Departure);
                return Task.CompletedTask;
            });
            Task.WaitAll(task1, task2);
            MergeTabs();
            MapModel.GenerateBmp(mapModel);
            
        }
        private void MergeTabs()
        {
            for (int i = 0; i < Forward.Image.GetLength(0); i++)
            {
                for (int j = 0; j < Forward.Image.GetLength(1); j++)
                {
                    if (Forward.Image[i, j] == ' ')
                    {
                        Forward.Image[i, j] = Backward.Image[i, j];
                    }
                }
            }
            mapModel = Forward.Image;
        }
    }
}
