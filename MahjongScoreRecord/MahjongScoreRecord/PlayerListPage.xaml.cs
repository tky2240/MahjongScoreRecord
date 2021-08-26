﻿using MahjongScoreRecord.Models;
using SQLite;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerListPage : ContentPage {
        public PlayerListPage() {
            InitializeComponent();
        }

        private void PlayerListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            ListView listView = (ListView)sender;
            Player selectedPlayer = (Player)listView.SelectedItem;
            listView.SelectedItem = null;
            DisplayAlert("Tapped", selectedPlayer.PlayerName, "OK");
            //Navigation.PushModalAsync(new NavigationPage(new )
        }

        private async void RegisterPlayerButton_Clicked(object sender, EventArgs e) {
            string playerName = await DisplayPromptAsync("雀士名", "登録する雀士の名前を入力してください", "OK", "Cancel", "hoge", 64, Keyboard.Text, "");
            if (playerName == null) {
                return;
            }
            if (Regex.IsMatch(playerName, @"^\s*$")) {
                await DisplayAlert("エラー", "正しい名前を入力してください", "OK");
                return;
            }
            SQLiteConnection db = await DBOperations.ConnectDB();
            db.Insert(new Player { PlayerName = playerName.Trim() });
            RefreshList(db);
        }
        private async void PlayerListPage_Appearing(object sender, EventArgs e) {
            SQLiteConnection db = await DBOperations.ConnectDB();
            RefreshList(db);
        }
        private void RefreshList(SQLiteConnection db) {
            PlayerListView.ItemsSource = db.Table<Player>().ToList(); ;
            db.Dispose();
        }
    }
}