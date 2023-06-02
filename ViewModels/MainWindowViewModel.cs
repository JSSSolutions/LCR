using CommandBase; // dealing with all the ICommand parts of MVVM
using LCR.Models; // including the Model where the binding variables are stored

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;

namespace LCR.ViewModels
{
    class MainWindowViewModel
    {
        public MainWindowModel AppModel { get; set; } // the object that deals with the Model
        public CmdBase<string> Action { get; private set; }
        private MainWindow Win; // needed to close the application, and apparently, so the graph can be populated

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public MainWindowViewModel(MainWindow W) // parameter only needed to close the app
        {
            AppModel = new MainWindowModel("Welcome To LCR!");
            Action = new CmdBase<string>(TakeAction);
            Win = W;
        }

        // ********************************************************************

        private void Cancel()
        {            
            Win.Close(); // closing the application wia an instance of the window
        }

        // ********************************************************************

        void GetLeftRightPlayers(int T, ref int L, ref int R)
        { // getting the indices of the players to the left and right of the current player (passed in as reference parameters)
            L = T - 1;
            if (L < 0)
                L = AppModel.Players - 1;
            R = T + 1;
            if (R == AppModel.Players)
                R = 0;
        }

        // ####################################################################

        private int GetPlayersLeft(List<int> PLC)
        { // Counts how many players still have chips left -- not really how many players are left in the game
            int P = 0;

            foreach (int PL in PLC)
            {
                if (PL > 0)
                    P++;
            }

            return P;
        }

        // ********************************************************************

        private void PlayGames()
        { // Here's where the simulations take place, and the results are graphed.
            double[] Averages, XCoords, YCoords;
            int Average, CenterChips, G, Highest, LeftPlayer, Lowest, P, R, RightPlayer, Roll, Rolls, Turn, Total, Turns;
            List<int> PlayerListChips = new List<int>();
            Random rnd = new Random();

            if ((AppModel.Players < 3) || (AppModel.Games < 1))
            {
                MessageBox.Show("You need at least 3 players and at least 1 turn to play this game!");
                return;
            }

            Win.TheGraph.Reset();
            XCoords = new double[AppModel.Games];
            YCoords = new double[AppModel.Games];     
            Averages = new double[AppModel.Games];  
            Win.TheGraph.Plot.Title("LCR GAME SIMULATION STATISTICS FOR " + AppModel.Players.ToString() + " AND " + AppModel.Games.ToString() + " GAMES");
            Win.TheGraph.Plot.XLabel("GAMES");
            Win.TheGraph.Plot.YLabel("TURNS");
            Average = 0;
            Highest = 0;
            Total = 0;
            Lowest = 0; // to be changed after first game

            for (G = 0; G < AppModel.Games; G++)
            {
                for (P = 0; P < AppModel.Players; P++)
                {
                    PlayerListChips.Add(3); // three chips for each player
                }

                LeftPlayer = -1;
                RightPlayer = -1; // both these need to be initialized

                Turns = 0;
                CenterChips = 0;
                Turn = rnd.Next() % AppModel.Players; // whose turn will be between 0 and # of players -1
                GetLeftRightPlayers(Turn, ref LeftPlayer, ref RightPlayer);
                P = GetPlayersLeft(PlayerListChips);
                while (P > 1) // game finishes when only one player has chips left
                {
                    Rolls = PlayerListChips[Turn];
                    if (Rolls > 3)
                        Rolls = 3;
                    for (R = 0; R < Rolls; R++)
                    {
                        Roll = rnd.Next() % 6;
                        if (Roll < 3) // 3, 4, or 5 = do nothing
                        {
                            PlayerListChips[Turn]--;
                            if (Roll == 0) // left
                                PlayerListChips[LeftPlayer]++;
                            if (Roll == 1) // center
                                CenterChips++;
                            if (Roll == 2) // right
                                PlayerListChips[RightPlayer]++;
                        }
                    }
                    if (Rolls > 0)
                        Turns++;
                    Turn++;
                    if (Turn == AppModel.Players)
                        Turn = 0;
                    GetLeftRightPlayers(Turn, ref LeftPlayer, ref RightPlayer);
                    P = GetPlayersLeft(PlayerListChips);
                }

                XCoords[G] = G + 1;
                YCoords[G] = Turns;
                
                Total += Turns;
                if (G==0)
                {
                    Highest = Turns;
                    Lowest = Turns;
                    Average = Turns;
                }
                else
                {
                    if (Turns < Lowest)
                        Lowest = Turns;
                    if (Turns > Highest)
                        Highest = Turns;
                    Average = Total / (G + 1);
                }
                PlayerListChips.Clear();
            }

            for (int x=0; x<AppModel.Games; x++)
            {
                Averages[x] = Average; // preparing the average turns line
            }
            
            Win.TheGraph.Plot.AddAnnotation("Average # Of Turns: " + Average.ToString() + "\nMost # Of Turns: " + Highest.ToString() + "\nLeast # Of Turns: " + Lowest.ToString());
            Win.TheGraph.Plot.AddScatter(XCoords, YCoords, color: System.Drawing.Color.Red);
            Win.TheGraph.Plot.AddScatter(XCoords, Averages, color: System.Drawing.Color.Black);

            Win.TheGraph.Refresh();
            AppModel.Status = "GAMES OVER";
        }

        // ********************************************************************

        public void TakeAction(string DB)
        {
            AppModel.Status = "Action Taken: " + DB;

            if (DB == "PLAY GAMES")
                PlayGames();

            if (DB == "CANCEL")
                Cancel();
        }
    }
}