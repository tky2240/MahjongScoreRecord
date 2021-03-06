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
    public partial class RecordUpdatePage : ContentPage {
        private readonly List<Picker> _PlayerPickers;
        private readonly FourPlayersRecord _FourPlayersRecord;
        private readonly ThreePlayersRecord _ThreePlayersRecord;
        public RecordUpdatePage(List<Player> players, FourPlayersRecord fourPlayersRecord) {
            InitializeComponent();
            _FourPlayersRecord = fourPlayersRecord;
            RecordNameEntry.Text = _FourPlayersRecord.RecordName;
            _PlayerPickers = new List<Picker>() { PlayerPicker1, PlayerPicker2, PlayerPicker3, PlayerPicker4 };
            _PlayerPickers.ForEach(picker => picker.ItemsSource = players);
            PlayerPicker1.SelectedItem = players.First(player => player.PlayerID == _FourPlayersRecord.PlayerID1);
            PlayerPicker2.SelectedItem = players.First(player => player.PlayerID == _FourPlayersRecord.PlayerID2);
            PlayerPicker3.SelectedItem = players.First(player => player.PlayerID == _FourPlayersRecord.PlayerID3);
            PlayerPicker4.SelectedItem = players.First(player => player.PlayerID == _FourPlayersRecord.PlayerID4);
            _ThreePlayersRecord = new ThreePlayersRecord();
        }
        public RecordUpdatePage(List<Player> players, ThreePlayersRecord threePlayersRecord) {
            InitializeComponent();
            _ThreePlayersRecord = threePlayersRecord;
            RecordNameEntry.Text = _ThreePlayersRecord.RecordName;
            _PlayerPickers = new List<Picker>() { PlayerPicker1, PlayerPicker2, PlayerPicker3};
            _PlayerPickers.ForEach(picker => picker.ItemsSource = players);
            PlayerPicker1.SelectedItem = players.First(player => player.PlayerID == _ThreePlayersRecord.PlayerID1);
            PlayerPicker2.SelectedItem = players.First(player => player.PlayerID == _ThreePlayersRecord.PlayerID2);
            PlayerPicker3.SelectedItem = players.First(player => player.PlayerID == _ThreePlayersRecord.PlayerID3);
            _FourPlayersRecord = new FourPlayersRecord();
        }
        private void RecordUpdatePage_Appearing(object sender, EventArgs e) {
            PlayerStackLayout4.BindingContext = Globals.GetCurrentPlayersMode();
        }
        private async void UpdateButton_Clicked(object sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(RecordNameEntry.Text)) {
                if (!_PlayerPickers.Any(picker => picker.SelectedItem == null)) {
                    using(SQLiteConnection db = await DBOperations.ConnectDB()) {
                        if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                            db.Update(new FourPlayersRecord {
                                RecordID = _FourPlayersRecord.RecordID,
                                PlayerID1 = ((Player)PlayerPicker1.SelectedItem).PlayerID,
                                PlayerID2 = ((Player)PlayerPicker2.SelectedItem).PlayerID,
                                PlayerID3 = ((Player)PlayerPicker3.SelectedItem).PlayerID,
                                PlayerID4 = ((Player)PlayerPicker4.SelectedItem).PlayerID,
                                RecordName = RecordNameEntry.Text.Trim(),
                                RecordTime = _FourPlayersRecord.RecordTime
                            });
                        }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                            db.Update(new ThreePlayersRecord {
                                RecordID = _ThreePlayersRecord.RecordID,
                                PlayerID1 = ((Player)PlayerPicker1.SelectedItem).PlayerID,
                                PlayerID2 = ((Player)PlayerPicker2.SelectedItem).PlayerID,
                                PlayerID3 = ((Player)PlayerPicker3.SelectedItem).PlayerID,
                                RecordName = RecordNameEntry.Text.Trim(),
                                RecordTime = _ThreePlayersRecord.RecordTime
                            });
                        }
                    }
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
        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}