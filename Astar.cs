using SuperLinq.Collections;
using System.Diagnostics;
namespace Pathfinding_Astar
{
    class Astar
    {
        private (int x, int y) Departure;
        private (int x, int y) Arrival;
        private static int _Size = MapModel._Size;
        public char[,] Image = new char[_Size, _Size];
        public Node[,] Grid = new Node[_Size, _Size];
        public UpdatablePriorityQueue<Node, double> OpenSet = new UpdatablePriorityQueue<Node, double>();
        private Node _Current;
        private ulong calculs = 0;
        private Stopwatch _StopWatch = null;
        private static object Lock = new object();
        private static Node value;
        private static bool IsEntered = false;
        public bool IsWay { get; private set; }

        public Astar((int x, int y) departure, (int x, int y) end)
        {
            Departure = departure;
            Arrival = end;
            Node startNode = new Node(Departure.x, Departure.y);
            OpenSet.Enqueue(startNode, startNode.FCost);
            Grid[Departure.x, Departure.y] = startNode;
            startNode.GCost = 0;
            startNode.FCost = HCostCalculate(Departure, Arrival);
            Image = (char[,])BidirectionnalAstar.mapModel.Clone();
            _StopWatch = new Stopwatch();
            _StopWatch.Start();
            value = new Node();
            Path();
        }

        public void Path()
        {
            while (OpenSet.Count > 0)
            {
                _Current = OpenSet.Dequeue();

                Image[_Current.X, _Current.Y] = 'V';
                bool check;
                
                lock (Lock)
                {
                    check = BidirectionnalAstar.MergedGrids.TryGetValue((_Current.X, _Current.Y), out value);
                    if (check)
                    {
                        ReconstructPath(value);
                        IsWay = true;
                        break;
                    }
                    BidirectionnalAstar.MergedGrids.Add((_Current.X, _Current.Y), _Current);
                }
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if ((i, j) != (0, 0))
                        {
                            int newX = _Current.X + i;
                            int newY = _Current.Y + j;

                            if (MapModel.IsTraversable(Image, newX, newY) && Image[newX, newY] != 'V')
                            {
                                ProcessNeighbor(newX, newY);
                                calculs++;
                            }

                        }
                    }
                }
            }

            _StopWatch.Stop();
            if (!IsWay)
                Console.WriteLine("Chemin impossible à réaliser");
            Console.WriteLine("Nodes calculés : " + calculs);
            Console.WriteLine("Temps du chemin : " + _StopWatch.ElapsedMilliseconds + " ms");
        }
        private void ProcessNeighbor(int x, int y)
        {
            Node neighbor;
            if (Grid[x, y] is null)
            {
                neighbor = new Node(x, y);
                Grid[x, y] = neighbor;
            }
            else
            {
                neighbor = Grid[x, y];
            }

            int newGCost = _Current.GCost + ((x != _Current.X && y != _Current.Y) ? 14 : 10);
            if (newGCost < neighbor.GCost)
            {
                neighbor.GCost = newGCost;
                neighbor.Parent = _Current;
                neighbor.FCost = neighbor.GCost + HCostCalculate((x, y), Arrival);
                OpenSet.Enqueue(neighbor, neighbor.FCost);

            }
        }

        private static double HCostCalculate((int x, int y) depart, (int x, int y) arrival)
        {
            int dx = Math.Abs(depart.x - arrival.x);
            int dy = Math.Abs(depart.y - arrival.y);
            return 10 * Math.Max(dx, dy);
        }
        private void ReconstructPath(Node value)
        {
            while ((_Current.X, _Current.Y) != Departure)
            {
                Image[_Current.X, _Current.Y] = 'P';
                _Current = _Current.Parent;
            }

            while ((value.X, value.Y) != Arrival)
            {
                Image[value.X, value.Y] = 'P';
                value = value.Parent;
            }

        }
    }

}