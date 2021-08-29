using MahjongScoreRecord.Models;
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
        private async void PlayerListPage_Appearing(object sender, EventArgs e) {
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                PlayerListView.ItemsSource = db.Table<Player>().ToList();
            }
        }
        private async void PlayerListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            ListView playerListView = (ListView)sender;
            Player selectedPlayer = (Player)playerListView.SelectedItem;
            if(selectedPlayer == null) {
                return;
            }
            playerListView.SelectedItem = null;
            await Navigation.PushModalAsync(new NavigationPage(new PlayerDetailPage(selectedPlayer.PlayerID)), true);
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
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                if(db.Insert(new Player { PlayerName = playerName.Trim() }) != 1) {
                    await DisplayAlert("エラー", "雀士の追加に失敗しました\nもう一度試してみてください", "OK");
                }
                PlayerListView.ItemsSource = db.Table<Player>().ToList();
            }
        }
    }
}