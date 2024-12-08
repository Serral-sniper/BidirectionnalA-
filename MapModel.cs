
using System.Drawing;
using SkiaSharp;

namespace Pathfinding_Astar
{
    abstract class MapModel
    {
        private static readonly object Lock = new();
        public const int Size = 150;

        // Image représentant la carte
        private static char[,] image = new char[Size + 1,Size + 1];
        // Point de départ
        public static (int x, int y) _Departure { get; private set; }
        // Point d'arrivée
        public static (int x, int y) _End { get; private set; }
        private static readonly SKBitmap ImageCanvas = new (new SKImageInfo(Size, Size));

        // Génère la carte avec un nombre spécifié de murs
        public static char[,] mapGeneration(int Wall)
        {
            SetBlankSpaces();
            GenerateSquare();
            DeparturePoint();
            EndPoint();
            WallGeneration(Wall);
            return image;
        }

        // Définit le point de départ aléatoirement
        private static void DeparturePoint()
        {
            int x = new Random().Next(1, Size - 2);
            int y = new Random().Next(1, Size - 2);
            image[x, y] = 'D';
            _Departure = (x, y);
        }

        // Définit le point d'arrivée aléatoirement
        private static void EndPoint()
        {
            int x = new Random().Next(1, Size - 2);
            int y = new Random().Next(1, Size - 2);
            image[x, y] = 'E';
            _End = (x, y);
        }

        // Réinitialise l'image en mettant tous les pixels en transparent
        private static void SetBlankSpaces()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    image[i, j] = ' ';
                }
            }
        }

        // Génère les bordures de la carte
        private static void GenerateSquare()
        {
            for (int i = 0; i < image.GetLength(0) - 1; i++)
            {
                image[i, 0] = 'B';
                image[i, Size - 1] = 'B';
                image[0, i] = 'B';
                image[Size - 1, i] = 'B';
            }
        }

        // Vérifie si une cellule est traversable
        public static bool IsTraversable(char[,] imageCP, int x, int y)
        {
            return imageCP[x, y] != 'B' && imageCP[x, y] != 'W';
        }

        private static void WallGeneration(int Wall)
        {
            for (int i = 0; i < Wall; i++)
            {
                int x = new Random().Next(1, Size - 1);
                int y = new Random().Next(1, Size - 1);
                if ((x, y) != _Departure && (x, y) != _End)
                {
                    image[x, y] = 'W';
                }
            }
        }
        
        public static void GenerateBmp(char[,] imageRawData)
        {
            lock (Lock)
            {
                BuildImageData(imageRawData);
                
                ImageCanvas.SetPixel(_Departure.x, _Departure.y, SKColors.Yellow);
                ImageCanvas.SetPixel(_End.x, _End.y, SKColors.Yellow);

                ExportImageData();
            }
        }

        private static void BuildImageData(char[,] imageRaw)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    SetPixel(imageRaw, i, j);
                }
            }
        }

        private static void SetPixel(char[,] imageRawData, int i, int j)
        {
            switch (imageRawData[i, j])
            {
                case 'B':
                    ImageCanvas.SetPixel(i, j, SKColors.White);
                    break;
                case 'W':
                    ImageCanvas.SetPixel(i, j, SKColors.Silver);
                    break;
                case 'V':
                    ImageCanvas.SetPixel(i, j, SKColors.Red);
                    break;
                case 'N':
                    ImageCanvas.SetPixel(i, j, SKColors.Green);
                    break;
                case 'P':
                    ImageCanvas.SetPixel(i, j, SKColors.Blue);
                    break;
                default:
                    ImageCanvas.SetPixel(i, j, SKColors.Empty);
                    break;

            }
        }

        private static void ExportImageData()
        {
            string workingDirectory = Environment.CurrentDirectory;

            DirectoryInfo? immediateParent = Directory.GetParent(workingDirectory);
            if (immediateParent?.Parent?.Parent is null)
            {
                return;
            }
                
            string projectDirectory = immediateParent.Parent.Parent.FullName;
            string outputDirectory = Path.Combine(projectDirectory, "output");

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string outputFile = Path.Combine(outputDirectory, "output.png");
                
            SKWStream fileStream = SKFileWStream.OpenStream(outputFile);
            ImageCanvas.Encode(fileStream, SKEncodedImageFormat.Png, 100);
        }
    }
}
