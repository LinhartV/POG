using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/*
0
0
0
2
1
0
2
5
0

1
1
1
100
-1
20

 */

namespace POG_semestralka
{
    class Point
    {
        public double x;
        public double y;
        public double z;

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


    }
    internal class Program
    {
        static void Main(string[] args)
        {
            //Nastavení desetinné tečky místo čárky
            System.Globalization.CultureInfo customCulture = new System.Globalization.CultureInfo("cs-CZ");
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //Seznam kontrolních bodů
            List<Point> points = new List<Point>();

            //Načítání bodů pro spline
            Console.WriteLine("Zadej postupně řídící body pro Coonsovu křivku (minimálně tři). Bod je určen vždy třemi souřadnicemi X, Y a Z.");
            while (true)
            {
                Console.WriteLine("Souřadnice bodu " + (points.Count + 1) + ":");
                points.Add(CreateNewPoint());
                if (points.Count > 2)
                {
                    Console.WriteLine("Pokud si přeješ zadat další bod, napiš Y. Pokud jsou všechny tvé body zadány, stiskni enter");
                    string answer = Console.ReadLine();
                    if (answer != "Y")
                    {
                        break;
                    }
                }
            }

            Point vectorK;
            Point vectorS;
            while (true)
            {

                Console.WriteLine("Zadej vektor 'k' pomocí tří souřadnic");
                vectorK = CreateNewPoint();
                Console.WriteLine("Zadej vektor 's' pomocí tří souřadnic");
                vectorS = CreateNewPoint();
                double ks = vectorK.x * vectorS.x + vectorK.y * vectorS.y + vectorK.z * vectorS.z;
                if (ks != 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Vektory nemohou být kolmé. Zadej vektory znovu.");
                }
            }

            //Promítnutí kontrolních bodů na rovinu
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = ProjectPoint(points[i], vectorK, vectorS);
            }

            Console.WriteLine("-----------------");
            Console.WriteLine("Výstup pro Latex:");
            Console.WriteLine("-----------------\n");
            //Výstup křivky pro Latex pstricks
            CreateCoonsCubicSpline(points);
            Console.ReadKey();
        }
        /// <summary>
        /// Vytvoření Coonsovy kubické křivky podle zadaných bodů. Vypíše příkazy pro Latex.
        /// </summary>
        /// <param name="points">Seznam bodů</param>
        static void CreateCoonsCubicSpline(List<Point> points)
        {
            List<string> spline = new List<string>();
            if (points.Count == 3)
            {
                points.Add(points[0]);
            }
            for (int i = 0; i < points.Count - 3; i++)
            {
                string x1 = "\n1 t -1 mul add 3 exp 6 div " + points[i].x + " mul %x1 \n";
                string x2 = "t 3 exp 2 div t 2 exp -1 mul add 2 3 div add " + points[i + 1].x + " mul add %x2 \n";
                string x3 = "t 3 exp -2 div t 2 exp 2 div add t 2 div add 1 6 div add " + points[i + 2].x + " mul add %x3 \n";
                string x4 = "t 3 exp 6 div " + points[i + 3].x + " mul add %x4 \n";

                string y1 = "1 t -1 mul add 3 exp 6 div " + points[i].y + " mul %y1 \n";
                string y2 = "t 3 exp 2 div t 2 exp -1 mul add 2 3 div add " + points[i + 1].y + " mul add %y2 \n";
                string y3 = "t 3 exp -2 div t 2 exp 2 div add t 2 div add 1 6 div add " + points[i + 2].y + " mul add %y3 \n";
                string y4 = "t 3 exp 6 div " + points[i + 3].y + " mul add %y4 \n";
                spline.Add(@"\parametricplot[linewidth=0.5pt,linecolor=black]{0}{1}{" + x1 + x2 + x3 + x4 + y1 + y2 + y3 + y4 + @"}");
            }
            Console.WriteLine(string.Join("\n\n", spline));
        }
        /// <summary>
        /// Načte data od uživatele pro vytvoření nového bodu
        /// </summary>
        static Point CreateNewPoint()
        {
            double x = ReadCoordinate("Souřadnice x: ");
            double y = ReadCoordinate("Souřadnice y: ");
            double z = ReadCoordinate("Souřadnice z: ");
            return new Point(x, y, z);
        }

        /// <summary>
        /// Načte hodnotu souřadnice s ošetřením vstupu
        /// </summary>
        static double ReadCoordinate(string text)
        {
            double output;
            while (true)
            {
                Console.WriteLine(text);
                string input = Console.ReadLine();
                bool ok = double.TryParse(input, out output);
                if (!ok)
                {
                    Console.WriteLine("Nesprávný vstup, znovu prosím.");
                }
                else
                    break;
            }
            return output;
        }

        /// <summary>
        /// Promítne bod na plochu rovnoběžnou s vektorem 'k' procházející bodem 0,0,0 ve směru 's'
        /// </summary>
        /// <param name="p">Bod k promítnutí</param>
        /// <param name="k">Vektor kolmý na plochu promítání</param>
        /// <param name="s">Směr promítání</param>
        /// <returns></returns>
        static Point ProjectPoint(Point p, Point k, Point s)
        {
            double ks = k.x * s.x + k.y * s.y + k.z * s.z;
            return new Point(p.x * (1 - k.x * s.x / ks) - p.y * (k.y * s.x / ks) - p.z * k.z * s.x / ks, -p.x * (k.x * s.y / ks) + p.y * (1 - k.y * s.y / ks) - p.z * k.z * s.y / ks, -p.x * (k.x * s.z / ks) - p.y * k.y * s.z / ks + p.z * (1 - k.z * s.z / ks));
        }
    }
}
