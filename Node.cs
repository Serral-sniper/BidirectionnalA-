

namespace Pathfinding_Astar
{
    class Node : IEquatable<Node>
    {
        // Coordonnée X du nœud
        public int X { get; private set; }
        // Coordonnée Y du nœud
        public int Y { get; private set; }
        // Coût G : coût du chemin du point de départ à ce nœud
        public int GCost { get; set; }
        // Coût F : somme du coût G et de l'estimation du coût jusqu'à la destination (heuristique)
        public double FCost { get; set; }
        // Nœud parent dans le chemin
        public Node Parent { get; set; }

        // Constructeur du nœud
        public Node(int x, int y)
        {
            X = x;
            Y = y;
            GCost = int.MaxValue;
            FCost = double.MaxValue;
            Parent = this;
        }
        public Node() { }

        public bool Equals(Node? other) => X == other?.X && Y == other.Y;
    }
}
