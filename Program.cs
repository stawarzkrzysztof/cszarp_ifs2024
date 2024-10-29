namespace cszarp4;

class Program {
    #region shuffle_function
    static void shuffle(List<string> deck) {
        Random rand = new Random();
        for (int i = deck.Count - 1; i > 0; i--) {
            int j = rand.Next(i + 1); // Get a random index from 0 to i
            // Swap elements at index i and j
            string temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }
    #endregion

    static void Main(string[] args) {
        Dictionary<string, int> card_mapper = new Dictionary<string, int>();
        List<string> talia = new List<string>();
        string[] specials = { "walet", "krolowa", "krol", "as" };
        int[] specials_vals = { 2, 3, 4, 11 };

        // make deck
        for (int i = 2; i < 15; i++) {
            if (i <= 10) {
                for (int j = 0; j < 4; j++) { talia.Add(Convert.ToString(i)); }
                card_mapper.Add(Convert.ToString(i), i);
            }
            else {
                for (int j = 0; j < 4; j++) { talia.Add(specials[j]); }
                card_mapper.Add(specials[i % 11], specials_vals[i % 11]);
            }
        }

        Console.WriteLine("-- Witaj w grze Oczko! -- \n Czy chcesz zagrać (1) czy wyjść (2) ?");
        string wanna_play = Console.ReadLine();

        while (wanna_play != "1" && wanna_play != "2") {
            Console.WriteLine("Nie ma takiej liczby, try again");
            wanna_play = Console.ReadLine();
        }

        bool wanna_play_again = wanna_play == "1";

        while (wanna_play_again) {
            shuffle(talia);
            List<string> player_hand = new List<string>();
            int player_sum = 0;
            int computer_sum = 0;

            string decision = "1";
            while (decision == "1") {
                // player gets a card
                string player_card = talia[talia.Count - 1];
                player_hand.Add(player_card);
                talia.RemoveAt(talia.Count - 1);
                int player_value = card_mapper[player_card];
                player_sum += player_value;

                // computer gets a card but dont print it
                string computer_card = talia[talia.Count - 1];
                talia.RemoveAt(talia.Count - 1);
                int computer_value = card_mapper[computer_card];
                computer_sum += computer_value;

                Console.WriteLine($"\nKarta, którą dostajesz to: {player_card}");
                Console.Write("( Twoja obecna ręka: ");
                foreach (string card in player_hand) {
                    Console.Write(card + ", ");
                }
                Console.WriteLine($"\nTwoja obecna suma punktów: {player_sum} ) \n");

                if (player_sum > 21) {
                    Console.WriteLine("Przegrałeś, przekroczyłeś 21!");
                    break;
                }

                Console.WriteLine("Czy chcesz grać dalej (1), oddać ruch przeciwnikowi (2) czy wyjść z gry (3)?");
                decision = Console.ReadLine();
                while (decision != "1" && decision != "2" && decision != "3") {
                    Console.WriteLine("Nie ma takiego wyboru, wybierz 1, 2 lub 3!");
                    decision = Console.ReadLine();
                }
            }

            if (player_sum <= 21 && decision == "2") {
                Console.WriteLine($"\nSuma punktów komputera: {computer_sum}");

                if (computer_sum > 21) {
                    Console.WriteLine("Komputer przegrał, przekroczył 21! Wygrywasz!");
                }
                else if (computer_sum >= player_sum) {
                    Console.WriteLine("Komputer wygrywa!");
                }
                else {
                    Console.WriteLine("Wygrywasz!");
                }
            }

            // replay asked
            Console.WriteLine("\nCzy chcesz zagrać ponownie? (tak/nie)");
            string play_again = Console.ReadLine();
            if (play_again.ToLower() != "tak") {
                wanna_play_again = false;
            }

        }

        Console.WriteLine("Dziękujemy za grę!");
    }
}
