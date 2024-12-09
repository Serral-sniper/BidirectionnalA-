using SuperLinq.Collections;
using System.Diagnostics;

namespace Pathfinding_Astar
{
    class Astar
    {
        private readonly (int x, int y) _departure;
        private readonly (int x, int y) _arrival;
        private const int Size = MapModel.Size;
        public readonly char[,] Image;
        public readonly Node[,] Grid = new Node[Size, Size];
        public readonly UpdatablePriorityQueue<Node, double> OpenSet = new();
        private Node _current;
        private ulong _calculs = 0;
        private readonly Stopwatch _stopWatch;
        private static readonly object _lock = new();
        private static Node _value = null!;
        private static bool _isEntered = false;
        public bool IsWay { get; private set; }

        public Astar((int x, int y) departure, (int x, int y) end)
        {
            _departure = departure;
            _arrival = end;
            Node startNode = new Node(_departure.x, _departure.y);
            OpenSet.Enqueue(startNode, startNode.FCost);
            Grid[_departure.x, _departure.y] = startNode;
            startNode.GCost = 0;
            startNode.FCost = HCostCalculate(_departure, _arrival);
            Image = (char[,])BidirectionnalAstar.MapModel.Clone();
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
            _value = new Node();
            Path();
        }

        public void Path()
        {
            while (OpenSet.Count > 0)
            {
                _current = OpenSet.Dequeue();

                Image[_current.X, _current.Y] = 'V';
                bool check;

                lock (_lock)
                {
                    check = BidirectionnalAstar.MergedGrids.TryGetValue((_current.X, _current.Y), out _value);
                    if (check)
                    {
                        ReconstructPath(_value);
                        IsWay = true;
                        break;
                    }

                    BidirectionnalAstar.MergedGrids.Add((_current.X, _current.Y), _current);
                }

                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if ((i, j) == (0, 0))
                            continue;
                        int newX = _current.X + i;
                        int newY = _current.Y + j;

                        if (!MapModel.IsTraversable(Image, newX, newY))
                            continue;

                        ProcessNeighbor(newX, newY);
                        _calculs++;
                    }
                }
            }

            _stopWatch.Stop();
            if (!IsWay)
                Console.WriteLine("Chemin impossible à réaliser");
            Console.WriteLine("Nodes calculés : " + _calculs);
            Console.WriteLine("Temps du chemin : " + _stopWatch.ElapsedMilliseconds + " ms");
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

            int newGCost = _current.GCost + ((x != _current.X && y != _current.Y) ? 14 : 10);
            if (newGCost < neighbor.GCost)
            {
                neighbor.GCost = newGCost;
                neighbor.Parent = _current;
                neighbor.FCost = neighbor.GCost + HCostCalculate((x, y), _arrival);
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
            // Console.WriteLine($"Reconstructing path with {path} and {path2}");
            // Console.WriteLine($"from {_departure.x}, {_departure.y} to {_arrival.x}, {_arrival.y}");
            //
            while ((_current.X, _current.Y) != _departure)
            {
                Image[_current.X, _current.Y] = 'P';
                _current = _current.Parent;
            }

            while ((value.X, value.Y) != _arrival)
            {
                Image[value.X, value.Y] = 'P';
                value = value.Parent;
            }
        }
    }
}