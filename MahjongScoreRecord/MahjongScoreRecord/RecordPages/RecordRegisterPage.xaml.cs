using MahjongScoreRecord.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordRegisterPage : ContentPage {
        private readonly List<Picker> _PlayerPickers;
        private readonly List<Player> _Players;
        public RecordRegisterPage(List<Player> players) {
            InitializeComponent();
            _PlayerPickers = new List<Picker>() { PlayerPicker1, PlayerPicker2, PlayerPicker3, PlayerPicker4 };
            _PlayerPickers.ForEach(picker => picker.ItemsSource = players);
            _Players = players;
        }

        private async void RegisterButton_Clicked(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(RecordNameEntry.Text)) {
                if (!_PlayerPickers.Any(picker => picker.SelectedItem == null)) {
                    SQLiteConnection db = await DBOperations.ConnectDB();
                    db.Insert(new FourPlayersRecord {
                        PlayerID1 = ((Player)PlayerPicker1.SelectedItem).PlayerID,
                        PlayerID2 = ((Player)PlayerPicker2.SelectedItem).PlayerID,
                        PlayerID3 = ((Player)PlayerPicker3.SelectedItem).PlayerID,
                        PlayerID4 = ((Player)PlayerPicker4.SelectedItem).PlayerID,
                        RecordName = RecordNameEntry.Text.Trim(),
                        RecordTime = DateTime.Now
                    });
                    await Navigation.PopModalAsync(true);
                    return;
                } else {
                    await DisplayAlert("エラー", "プレイヤー名が未選択です", "OK");
                }
            } else {
                await DisplayAlert("エラー", "対局名が空欄です", "OK");
            }
        }

        private void PlayerPicker_SelectedIndexChanged(object sender, EventArgs e) {
            Picker changedPicker = (Picker)sender;
            if (changedPicker.SelectedItem != null) {
                if (_PlayerPickers.Where(picker => picker != changedPicker).Any(picker => picker.SelectedItem == changedPicker.SelectedItem)) {
                    changedPicker.SelectedItem = null;
                }
            }
        }

        private async void RecordRegisterPage_Appearing(object sender, EventArgs e) {
            if (_Players.Count() < 4) {
                await DisplayAlert("エラー", "登録プレイヤーが4人未満です", "OK");
                await Navigation.PopModalAsync(true);
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}