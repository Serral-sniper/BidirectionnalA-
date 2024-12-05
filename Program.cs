using Pathfinding_Astar;
int wall = 0;
bool OK = false;

try
{
    Console.Write("Combien de mur voulez-vous sur la map? : ");
    wall = int.Parse(Console.ReadLine());
    Console.WriteLine();
    OK = true;
}
catch
{
    wall = 0;
}
Console.Clear();

BidirectionnalAstar bidirectionnalAstar = new BidirectionnalAstar(wall);
