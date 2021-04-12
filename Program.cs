using System;
using System.Collections.Generic;

namespace Golf
{
    class Program
    {
        static readonly int helLangd = 500;
        static readonly int maxSlag = 5;
        static double aterstaendeLangd;
        static Dictionary<int, Slag> slagLista = new();

        static void Main(string[] args)
        {
            bool swEjStopp;
            int itest = 0;
            double[,] testInput = null;
            double[,] testInput1 = new double[,] { { 30, 80 }, { 45, 60 }, { 30, 30 }, { 10, 5 }, { 10, 1 } }; // Korrekt
            double[,] testInput2 = new double[,] { { 30, 80 }, { 120, 60 } }; // Vinkel fel
            double[,] testInput3 = new double[,] { { 30, 80 }, { 45, 300 } }; // Hastighet fel
            double[,] testInput4 = new double[,] { { 30, 80 }, { 45, 60 }, { 30, 30 }, { 10, 30 }, { 10, 20 }, { 10, 1 } }; // För många slag
            if (args.Length > 0)
            {
                swEjStopp = true;
                switch (args[0])
                {
                    case "1": testInput = testInput1; break;
                    case "2": testInput = testInput2; break;
                    case "3": testInput = testInput3; break;
                    case "4": testInput = testInput4; break;
                    default: testInput = testInput1; break;
                }
            }
            else swEjStopp = false;
            //swEjStopp = false;   // Kör programmet normalt, med inmatningar
            //swEjStopp = true;   // Testkör programmet automatiskt, utan några inmatningar ..

            bool swSpelSlut = false;
            int idSlag = 0;
            aterstaendeLangd = helLangd;

            while (!swSpelSlut)
            {
                idSlag++;
                Slag slag = new(idSlag);
                if (swEjStopp)
                {
                    slag.Vinkel = (int)testInput[itest, 0];
                    slag.Hastighet = testInput[itest, 1];
                    itest++;
                }
                else
                {
                    Inmatning(slag);
                }
                try
                {
                    slag.BeraknaStracka();
                    slag.Kontrollera();
                    aterstaendeLangd = Math.Abs(aterstaendeLangd - slag.Stracka);
                    //Console.WriteLine(slag);
                    slagLista.Add(idSlag, slag);
                    Console.Write($"Slag {slag.IdSlag} Slagets längd {slag.Stracka:F} meter. ");
                    Console.WriteLine($"Återstående längd {aterstaendeLangd:F} meter");
                }
                catch
                {
                    swSpelSlut = true;
                }

                if (!swSpelSlut)
                {
                    if (aterstaendeLangd <= 0.1)
                    {
                        Console.WriteLine("Du lyckades!");
                        foreach (var ettSlag in slagLista)
                        {
                            Console.WriteLine($"Slag {ettSlag.Value.IdSlag} var {ettSlag.Value.Stracka:F} meter.");
                            //Console.WriteLine(ettSlag);
                        }
                        swSpelSlut = true;
                    }
                    else if (slagLista.Count >= maxSlag)
                    {
                        Console.WriteLine("Du slog för många slag!");
                        swSpelSlut = true;
                    }
                }
            }
            Console.WriteLine("Spelet slut (tryck valfri tangent för att avsluta körningen)");
            Console.ReadKey();
        }
        public static void Inmatning(Slag slag)
        {
            //string input;
            double talD;
            //int talI;

            Console.Write($"Slag {slag.IdSlag} - ");
            talD = InmatningTal("vinkel i grader");
            slag.Vinkel = Convert.ToInt32(talD);
            talD = InmatningTal("hastighet i m/s");
            slag.Hastighet = talD;
        }
        public static double InmatningTal(string text)
        {
            string input;
            double tal;

            Console.Write($"Mata in {text}: ");
            input = Console.ReadLine();
            if (input == "")
                tal = 0;
            else
                while (!double.TryParse(input, out tal))
                {
                    Console.Write($"Felaktigt värde. Ange {text}: ");
                    input = Console.ReadLine();
                }
            return tal;
        }
    }
    class Slag
    {
        private int idSlag;
        private double hastighet;
        private int vinkel;
        private double stracka;

        public Slag()
        {
            stracka = 0;
        }
        public Slag(int idSlag)
        {
            stracka = 0;
            this.idSlag = idSlag;
        }

        public int IdSlag { get => idSlag; set => idSlag = value; }
        public double Hastighet { get => hastighet; set => hastighet = value; }
        public int Vinkel { get => vinkel; set => vinkel = value; }
        public double Stracka { get => stracka; set => stracka = value; }

        public override string ToString()
        {
            string str;
            str = $"id:{idSlag}, hastighet:{hastighet}, vinkel:{vinkel}, sträcka:{stracka}.";
            return str;
        }

        internal void BeraknaStracka()
        {
            double tyngdacceleration = 9.8;

            double vinkelR = Math.PI * vinkel / 180.0;
            stracka = Math.Pow(hastighet, 2) / (Math.Sin(2 * vinkelR) * tyngdacceleration);
        }
        internal void Kontrollera()
        {
            int maxLangd = 1000;
            int maxVinkel = 90;
            if (vinkel > maxVinkel || vinkel < 0)
            {
                throw new CustomException("Du slog bakåt!");
            }
            if (stracka > maxLangd || stracka < 0)
            {
                throw new CustomException("Du slog utanför banan!");
            }
        }
    }
    class CustomException : Exception
    {
        public CustomException(string text)
        {
            Console.WriteLine(text);
        }
    }
}
