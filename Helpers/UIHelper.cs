using System;

namespace Team1_Code_Stars_Events.Helpers
{
    // Statisk klass med verktyg för att hantera konsolens utseende och inmatning
    public static class UIHelper
    {
        // --- DESIGN & FORMATTERING ---

        // Rensar skärmen och visar en tydlig rubrik
        public static void ShowHeader(string text)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n{text.ToUpper()}");
            Console.ResetColor();
            Console.WriteLine(new string('-', 35));
        }

        // Visar ett grönt bekräftelsemeddelande
        public static void ShowSuccess(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✅ {msg}");
            Console.ResetColor();
        }

        // Visar ett rött felmeddelande
        public static void ShowError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ {msg}");
            Console.ResetColor();
        }

        // --- INPUT & VALIDERING ---

        // Ser till att användaren inte lämnar ett fält tomt
        public static string GetValidString(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    ShowError("Du måste skriva något här.");
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        // Tvingar användaren att skriva in en giltig siffra
        public static int GetValidInt(string prompt)
        {
            int result;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    return result;
                }
                ShowError("Ogiltigt val. Ange en siffra.");
            }
        }

        // Pausar programmet så användaren hinner läsa resultatet
        public static void PressAnyKey()
        {
            Console.WriteLine("\nTryck på valfri tangent för att gå tillbaka...");
            Console.ReadKey();
        }
    }
}