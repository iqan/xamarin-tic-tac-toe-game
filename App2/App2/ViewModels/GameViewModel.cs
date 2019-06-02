using App2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace App2.ViewModels
{

    public class GameViewModel : BaseViewModel
    {
        private string _result;
        private bool _showResult;
        private Player _turn;

        public GameViewModel()
        {
            Title = "Game";
            Tiles = new ObservableCollection<Tile>();
            Play = new Command((tile) => ButtonClicked(tile));
            Reset = new Command(() => ResetClicked());
            InitializeGame();
        }

        private void InitializeGame()
        {
            ShowResult = false;
            Result = "";
            Turn = Player.O;
            Tiles.Clear();
            for (int i = 0; i < 9; i++)
            {
                Tiles.Add(new Tile { Position = i, PlayedBy = null, Enabled = true });
            }
        }

        public ObservableCollection<Tile> Tiles { get; set; }

        public Player Turn
        {
            get => _turn;
            set => SetProperty(ref _turn, value);
        }

        public Command Play { get; set; }

        public Command Reset { get; set; }

        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        public bool ShowResult
        {
            get => _showResult;
            set => SetProperty(ref _showResult, value);
        }

        void ResetClicked()
        {
            foreach (var item in Tiles)
            {
                item.Enabled = true;
                item.PlayedBy = null;
            }
            ShowResult = false;
            Result = "";
            Turn = Player.O;
        }

        void ButtonClicked(object tile)
        {
            Debug.WriteLine(tile as string);
            try
            {
                var playedTile = Convert.ToInt32(tile as string);
                if (!Tiles[playedTile].Enabled)
                {
                    return;
                }
                Tiles[playedTile].Enabled = false;
                Tiles[playedTile].PlayedBy = Turn;
                Turn = Turn == Player.O ? Player.X : Player.O;

                CheckWinner();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CheckWinner()
        {
            var movesForPlayerX = Tiles.Where(t => t.PlayedBy == Player.X).Select(t => t.Position + 1);
            var movesForPlayerO = Tiles.Where(t => t.PlayedBy == Player.O).Select(t => t.Position + 1);
            if (IsWinner(movesForPlayerX))
            {
                DeclareWinner("X");
            }
            if (IsWinner(movesForPlayerO))
            {
                DeclareWinner("O");
            }
        }

        private void DeclareWinner(string winner)
        {
            Result = $"Player {winner} won";
            Debug.WriteLine($"Declaring winner:{Result}");
            ShowResult = true;
            foreach (var item in Tiles)
            {
                item.Enabled = false;
            }
            MessagingCenter.Send(this, "DisplayResult", Result);
        }

        bool IsWinner(IEnumerable<int> moves)
        {
            Debug.WriteLine($"Player moves: {string.Join(" ", moves)}");
            return (moves.Contains(1) && moves.Contains(2) && moves.Contains(3)) ||
                (moves.Contains(4) && moves.Contains(5) && moves.Contains(6)) ||
                (moves.Contains(7) && moves.Contains(8) && moves.Contains(9)) ||
                (moves.Contains(1) && moves.Contains(4) && moves.Contains(7)) ||
                (moves.Contains(2) && moves.Contains(5) && moves.Contains(8)) ||
                (moves.Contains(3) && moves.Contains(6) && moves.Contains(9)) ||
                (moves.Contains(1) && moves.Contains(5) && moves.Contains(9)) ||
                (moves.Contains(3) && moves.Contains(5) && moves.Contains(7))
                ;
        }
    }
}
