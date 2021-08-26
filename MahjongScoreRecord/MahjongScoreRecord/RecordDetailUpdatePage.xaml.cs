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
    public partial class RecordDetailUpdatePage : ContentPage {
        private readonly List<Entry> _PlayerPointEntries;
        private readonly List<Picker> _WindPickers;
        private readonly int _RecordDetailID;

        public RecordDetailUpdatePage(int recordDetailID) {
            InitializeComponent();
            _RecordDetailID = recordDetailID;
            _PlayerPointEntries = new List<Entry>() { PlayerPoint1Entry, PlayerPoint2Entry, PlayerPoint3Entry, PlayerPoint4Entry };
            _WindPickers = new List<Picker>() { WindPicker1, WindPicker2, WindPicker3, WindPicker4 };
        }
        private async void RecordDetailUpdatePage_Appearing(object sender, EventArgs e) {
            using(SQLiteConnection db = await DBOperations.ConnectDB()){
                FourPlayersRecordDetail fourPlayersRecordDetail = db.Table<FourPlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID);
                FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == fourPlayersRecordDetail.RecordID);
                List<Player> players = db.Table<Player>().ToList();
                PlayerName1Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID1).PlayerName;
                PlayerName2Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID2).PlayerName;
                PlayerName3Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID3).PlayerName;
                PlayerName4Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID4).PlayerName;
                _WindPickers.ForEach(picker => picker.ItemsSource = Globals.winds);
                WindPicker1.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind1);
                WindPicker2.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind2);
                WindPicker3.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind3);
                WindPicker4.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind4);
                PlayerPoint1Entry.Text = fourPlayersRecordDetail.PlayerPoint1.ToString();
                PlayerPoint2Entry.Text = fourPlayersRecordDetail.PlayerPoint2.ToString();
                PlayerPoint3Entry.Text = fourPlayersRecordDetail.PlayerPoint3.ToString();
                PlayerPoint4Entry.Text = fourPlayersRecordDetail.PlayerPoint4.ToString();
                PlayerPoints playerPoints = new PlayerPoints(fourPlayersRecordDetail.PlayerPoint1, fourPlayersRecordDetail.PlayerPoint2, fourPlayersRecordDetail.PlayerPoint3, fourPlayersRecordDetail.PlayerPoint4);
                PlayerWinds playerWinds = new PlayerWinds((Winds)fourPlayersRecordDetail.PlayerWind1, (Winds)fourPlayersRecordDetail.PlayerWind2, (Winds)fourPlayersRecordDetail.PlayerWind3, (Winds)fourPlayersRecordDetail.PlayerWind4);
                AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds);
                AdjustmentScore1Label.BindingContext = adjustmentPoints.AdjustmentScore1;
                AdjustmentScore2Label.BindingContext = adjustmentPoints.AdjustmentScore2;
                AdjustmentScore3Label.BindingContext = adjustmentPoints.AdjustmentScore3;
                AdjustmentScore4Label.BindingContext = adjustmentPoints.AdjustmentScore4;
            }
        }

        private void PlayerPointEntry_TextChanged(object sender, TextChangedEventArgs e) {
            Entry playerPointEntry = (Entry)sender;
            if (!string.IsNullOrEmpty(playerPointEntry.Text)) {
                if (!Regex.IsMatch(playerPointEntry.Text, @"^-?\d+$")) {

                    if (Regex.IsMatch(playerPointEntry.Text, @"^-.*$")) {
                        playerPointEntry.Text = "-" + Regex.Replace(playerPointEntry.Text.Substring(1), @"[^\d]*", "");
                    } else {
                        playerPointEntry.Text = Regex.Replace(playerPointEntry.Text, @"[^\d]*", "");
                    }

                }
            }
        }

        private void PlayerPointEntry_Unfocused(object sender, FocusEventArgs e) {
            Entry playerPointEntry = (Entry)sender;
            if (int.TryParse(playerPointEntry.Text, out int point)) {
                playerPointEntry.Text = point.ToString();

                if (_PlayerPointEntries.All(entry => !string.IsNullOrEmpty(entry.Text))) {
                    if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == _WindPickers.Count()) {

                        PlayerPoints playerPoints = new PlayerPoints(int.Parse(PlayerPoint1Entry.Text),
                                                                     int.Parse(PlayerPoint2Entry.Text),
                                                                     int.Parse(PlayerPoint3Entry.Text),
                                                                     int.Parse(PlayerPoint4Entry.Text));
                        PlayerWinds playerWinds = new PlayerWinds(((KeyValuePair<Winds, string>)WindPicker1.SelectedItem).Key,
                                                                  ((KeyValuePair<Winds, string>)WindPicker2.SelectedItem).Key,
                                                                  ((KeyValuePair<Winds, string>)WindPicker3.SelectedItem).Key,
                                                                  ((KeyValuePair<Winds, string>)WindPicker4.SelectedItem).Key);
                        AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds);
                        AdjustmentScore1Label.BindingContext = adjustmentPoints.AdjustmentScore1;
                        AdjustmentScore2Label.BindingContext = adjustmentPoints.AdjustmentScore2;
                        AdjustmentScore3Label.BindingContext = adjustmentPoints.AdjustmentScore3;
                        AdjustmentScore4Label.BindingContext = adjustmentPoints.AdjustmentScore4;
                        return;
                    }
                }
            } else {
                playerPointEntry.Text = string.Empty;
            }
            AdjustmentScore1Label.BindingContext = null;
            AdjustmentScore2Label.BindingContext = null;
            AdjustmentScore3Label.BindingContext = null;
            AdjustmentScore4Label.BindingContext = null;
        }
        private void WindPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (_PlayerPointEntries.All(entry => !string.IsNullOrEmpty(entry.Text))) {
                if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == _WindPickers.Count()) {

                    PlayerPoints playerPoints = new PlayerPoints(int.Parse(PlayerPoint1Entry.Text),
                                                                 int.Parse(PlayerPoint2Entry.Text),
                                                                 int.Parse(PlayerPoint3Entry.Text),
                                                                 int.Parse(PlayerPoint4Entry.Text));
                    PlayerWinds playerWinds = new PlayerWinds(((KeyValuePair<Winds, string>)WindPicker1.SelectedItem).Key,
                                                              ((KeyValuePair<Winds, string>)WindPicker2.SelectedItem).Key,
                                                              ((KeyValuePair<Winds, string>)WindPicker3.SelectedItem).Key,
                                                              ((KeyValuePair<Winds, string>)WindPicker4.SelectedItem).Key);
                    AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds);
                    AdjustmentScore1Label.BindingContext = adjustmentPoints.AdjustmentScore1;
                    AdjustmentScore2Label.BindingContext = adjustmentPoints.AdjustmentScore2;
                    AdjustmentScore3Label.BindingContext = adjustmentPoints.AdjustmentScore3;
                    AdjustmentScore4Label.BindingContext = adjustmentPoints.AdjustmentScore4;
                    return;
                }
            }
            AdjustmentScore1Label.BindingContext = null;
            AdjustmentScore2Label.BindingContext = null;
            AdjustmentScore3Label.BindingContext = null;
            AdjustmentScore4Label.BindingContext = null;
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e) {
            if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() != 4) {
                await DisplayAlert("エラー", "風を正しく選択してください", "OK");
                return;
            }
            if (_PlayerPointEntries.Any(entry => string.IsNullOrEmpty(entry.Text))) {
                await DisplayAlert("エラー", "全得点を正しく入力してください", "OK");
                return;
            }
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                FourPlayersRecordDetail fourPlayersRecordDetail = db.Table<FourPlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID);
                db.Update(new FourPlayersRecordDetail() {
                    RecordDetailID = _RecordDetailID,
                    RecordID = fourPlayersRecordDetail.RecordID,
                    PlayerPoint1 = int.Parse(PlayerPoint1Entry.Text),
                    PlayerPoint2 = int.Parse(PlayerPoint2Entry.Text),
                    PlayerPoint3 = int.Parse(PlayerPoint3Entry.Text),
                    PlayerPoint4 = int.Parse(PlayerPoint4Entry.Text),
                    PlayerWind1 = (int)((KeyValuePair<Winds, string>)WindPicker1.SelectedItem).Key,
                    PlayerWind2 = (int)((KeyValuePair<Winds, string>)WindPicker2.SelectedItem).Key,
                    PlayerWind3 = (int)((KeyValuePair<Winds, string>)WindPicker3.SelectedItem).Key,
                    PlayerWind4 = (int)((KeyValuePair<Winds, string>)WindPicker4.SelectedItem).Key,
                    MatchCount = fourPlayersRecordDetail.MatchCount
                });
            }
            await Navigation.PopModalAsync(true);
        }
        private async void DeleteButton_Clicked(object sender, EventArgs e) {
            if (await DisplayAlert("削除確認", "この対局結果を削除しますか？", "Yes", "No")) {
                using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                    int matchCount = db.Table<FourPlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID).MatchCount;
                    db.Table<FourPlayersRecordDetail>().Delete(detail => detail.RecordDetailID == _RecordDetailID);
                    db.UpdateAll(
                        db.Table<FourPlayersRecordDetail>().Where(detail => detail.MatchCount > matchCount).Select(detail => new FourPlayersRecordDetail() {
                            RecordDetailID = detail.RecordDetailID,
                            RecordID = detail.RecordID,
                            PlayerPoint1 = detail.PlayerPoint1,
                            PlayerPoint2 = detail.PlayerPoint2,
                            PlayerPoint3 = detail.PlayerPoint3,
                            PlayerPoint4 = detail.PlayerPoint4,
                            PlayerWind1 = detail.PlayerWind1,
                            PlayerWind2 = detail.PlayerWind2,
                            PlayerWind3 = detail.PlayerWind3,
                            PlayerWind4 = detail.PlayerWind4,
                            MatchCount = detail.MatchCount - 1
                        }).ToList()
                    ); 
                }
                await Navigation.PopModalAsync(true);
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}