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
    public partial class RecordDetailRegisterPage : ContentPage {
        private readonly List<Entry> _PlayerPointEntries;
        private readonly List<Picker> _WindPickers;
        private readonly List<Label> _AdjustmentLabels;
        private readonly int _RecordID;
        public RecordDetailRegisterPage(int recordID) {
            InitializeComponent();
            _RecordID = recordID;
            _PlayerPointEntries = new List<Entry>() { PlayerPointEntry1, PlayerPointEntry2, PlayerPointEntry3, PlayerPointEntry4 };
            _WindPickers = new List<Picker>() { WindPicker1, WindPicker2, WindPicker3, WindPicker4 };
            _AdjustmentLabels = new List<Label>() { AdjustmentScoreLabel1, AdjustmentScoreLabel2, AdjustmentScoreLabel3, AdjustmentScoreLabel4 };
            _AdjustmentLabels.ForEach(label => label.BindingContext = null);
            int i = 0;
            _WindPickers.ForEach(picker => {
                picker.ItemsSource = Globals.winds;
                picker.SelectedIndex = i;
                i++;
            });
        }
        private async void RecordDetailRegisterPage_Appearing(object sender, EventArgs e) {
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == _RecordID);
                List<Player> players = db.Table<Player>().ToList();
                PlayerNameLabel1.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID1).PlayerName;
                PlayerNameLabel2.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID2).PlayerName;
                PlayerNameLabel3.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID3).PlayerName;
                PlayerNameLabel4.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID4).PlayerName;
            }
        }
        private void PlayerPointEntry_TextChanged(object sender, TextChangedEventArgs e) {
            Entry playerPointEntry = (Entry)sender;
            if (!string.IsNullOrWhiteSpace(playerPointEntry.Text)) {
                playerPointEntry.Text = PointStringConverter.DeleteNonNumericCharacterWithMinus(playerPointEntry.Text);
            }
        }

        private async void PlayerPointEntry_Unfocused(object sender, FocusEventArgs e) {
            Entry playerPointEntry = (Entry)sender;
            if (int.TryParse(playerPointEntry.Text, out int point)) {
                playerPointEntry.Text = point.ToString();
                if (_PlayerPointEntries.All(entry => !string.IsNullOrWhiteSpace(entry.Text))) {
                    if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == 4) {
                        using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                            ShowAdjustmentScore(db);
                        }
                        return;
                    }
                }
            } else {
                playerPointEntry.Text = string.Empty;
            }
            _AdjustmentLabels.ForEach(lable => lable.BindingContext = null);
        }
        private async void WindPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (_PlayerPointEntries.All(entry => !string.IsNullOrEmpty(entry.Text))) {
                if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == 4) {
                    using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                        ShowAdjustmentScore(db);
                    }
                    return;
                }
            }
            _AdjustmentLabels.ForEach(label => label.BindingContext = null);
        }
        private void ShowAdjustmentScore(SQLiteConnection db) {
            int bonusID = Globals.GetCurrentFourPlayersBonusID();
            FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
            PlayerPoints playerPoints = new PlayerPoints(int.Parse(PlayerPointEntry1.Text),
                                                 int.Parse(PlayerPointEntry2.Text),
                                                 int.Parse(PlayerPointEntry3.Text),
                                                 int.Parse(PlayerPointEntry4.Text));
            PlayerWinds playerWinds = new PlayerWinds(((KeyValuePair<Winds, string>)WindPicker1.SelectedItem).Key,
                                                      ((KeyValuePair<Winds, string>)WindPicker2.SelectedItem).Key,
                                                      ((KeyValuePair<Winds, string>)WindPicker3.SelectedItem).Key,
                                                      ((KeyValuePair<Winds, string>)WindPicker4.SelectedItem).Key);
            AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, fourPlayersBonus);
            AdjustmentScoreLabel1.BindingContext = adjustmentPoints.AdjustmentScore1;
            AdjustmentScoreLabel2.BindingContext = adjustmentPoints.AdjustmentScore2;
            AdjustmentScoreLabel3.BindingContext = adjustmentPoints.AdjustmentScore3;
            AdjustmentScoreLabel4.BindingContext = adjustmentPoints.AdjustmentScore4;
        }
        private async void RegisterButton_Clicked(object sender, EventArgs e) {
            if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() != _WindPickers.Count()) {
                await DisplayAlert("エラー", "風を正しく選択してください", "OK");
                return;
            }
            if (_PlayerPointEntries.Any(entry => string.IsNullOrEmpty(entry.Text))) {
                await DisplayAlert("エラー", "全得点を正しく入力してください", "OK");
                return;
            }
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                int bonusID = (int)Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()];
                FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                if(int.Parse(PlayerPointEntry1.Text) + int.Parse(PlayerPointEntry2.Text) + int.Parse(PlayerPointEntry3.Text) + int.Parse(PlayerPointEntry4.Text) != fourPlayersBonus.OriginPoint * 4) {
                    if(!await DisplayAlert("得点確認", "点数の合計が原点と等しくありませんがこのまま登録しますか？", "Yes", "No")) {
                        return;
                    }
                }
                int matchCount = 0;
                if (db.Table<FourPlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).Any()) {
                    matchCount = db.Table<FourPlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).Select(detail => detail.MatchCount).Max();
                }
                db.Insert(new FourPlayersRecordDetail() {
                    RecordID = _RecordID,
                    PlayerPoint1 = int.Parse(PlayerPointEntry1.Text),
                    PlayerPoint2 = int.Parse(PlayerPointEntry2.Text),
                    PlayerPoint3 = int.Parse(PlayerPointEntry3.Text),
                    PlayerPoint4 = int.Parse(PlayerPointEntry4.Text),
                    PlayerWind1 = (int)((KeyValuePair<Winds, string>)WindPicker1.SelectedItem).Key,
                    PlayerWind2 = (int)((KeyValuePair<Winds, string>)WindPicker2.SelectedItem).Key,
                    PlayerWind3 = (int)((KeyValuePair<Winds, string>)WindPicker3.SelectedItem).Key,
                    PlayerWind4 = (int)((KeyValuePair<Winds, string>)WindPicker4.SelectedItem).Key,
                    MatchCount = matchCount + 1
                });
            }
            await Navigation.PopModalAsync(true);
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}