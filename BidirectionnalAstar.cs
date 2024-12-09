namespace Pathfinding_Astar
{
    class BidirectionnalAstar
    {
        private Astar _forward;
        private Astar _backward;
        public static Dictionary<(int, int), Node> MergedGrids;
        //public static (ulong, ulong) Counter;
        public static char[,] MapModel;

        public BidirectionnalAstar(int walls)
        {
            MapModel = Pathfinding_Astar.MapModel.mapGeneration(walls);
            MergedGrids = new Dictionary<(int, int), Node>();
            //Counter = (0, 0);
            Path();
        }
        public void Path()
        {
            Task task1 = Task.Run(() => 
            {
                _forward = new Astar(Pathfinding_Astar.MapModel._Departure, Pathfinding_Astar.MapModel._End);
                return Task.CompletedTask;
            });
            Task task2 = Task.Run(() =>
            {
                _backward = new Astar(Pathfinding_Astar.MapModel._End, Pathfinding_Astar.MapModel._Departure);
                return Task.CompletedTask;
            });
            Task.WaitAll(task1, task2);
            
            MergeTabs();
            Pathfinding_Astar.MapModel.GenerateBmp(MapModel);
            
        }
        private void MergeTabs()
        {
            for (int i = 0; i < _forward.Image.GetLength(0); i++)
            {
                for (int j = 0; j < _forward.Image.GetLength(1); j++)
                {
                    ColorMap(i, j);
                }
            }
        }

        private void ColorMap(int i, int j)
        {
            if (_forward.Image[i, j] == 'P' || _backward.Image[i, j] == 'P')
            {
                MapModel[i, j] = 'P';
                return;
            }

            if (_forward.Image[i, j] == 'V' || _backward.Image[i, j] == 'V')
            {
                MapModel[i, j] = 'V';
                return;
            }

            MapModel[i, j] = _forward.Image[i, j];
        }
    }
}
