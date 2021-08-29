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
        private readonly List<Label> _AdjustmentLabels;
        private readonly int _RecordDetailID;

        public RecordDetailUpdatePage(int recordDetailID) {
            InitializeComponent();
            _RecordDetailID = recordDetailID;
            _PlayerPointEntries = new List<Entry>() { PlayerPointEntry1, PlayerPointEntry2, PlayerPointEntry3, PlayerPointEntry4 };
            _WindPickers = new List<Picker>() { WindPicker1, WindPicker2, WindPicker3, WindPicker4 };
            _AdjustmentLabels = new List<Label>() { AdjustmentScoreLabel1, AdjustmentScoreLabel2, AdjustmentScoreLabel3, AdjustmentScoreLabel4 };
        }
        private async void RecordDetailUpdatePage_Appearing(object sender, EventArgs e) {
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                FourPlayersRecordDetail fourPlayersRecordDetail = db.Table<FourPlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID);
                FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == fourPlayersRecordDetail.RecordID);
                int bonusID = (int)Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()];
                FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                List<Player> players = db.Table<Player>().ToList();
                PlayerNameLabel1.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID1).PlayerName;
                PlayerNameLabel2.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID2).PlayerName;
                PlayerNameLabel3.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID3).PlayerName;
                PlayerNameLabel4.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID4).PlayerName;
                _WindPickers.ForEach(picker => picker.ItemsSource = Globals.winds);
                WindPicker1.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind1);
                WindPicker2.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind2);
                WindPicker3.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind3);
                WindPicker4.SelectedItem = Globals.winds.First(wind => (int)wind.Key == fourPlayersRecordDetail.PlayerWind4);
                PlayerPointEntry1.Text = fourPlayersRecordDetail.PlayerPoint1.ToString();
                PlayerPointEntry2.Text = fourPlayersRecordDetail.PlayerPoint2.ToString();
                PlayerPointEntry3.Text = fourPlayersRecordDetail.PlayerPoint3.ToString();
                PlayerPointEntry4.Text = fourPlayersRecordDetail.PlayerPoint4.ToString();
                PlayerPoints playerPoints = new PlayerPoints(fourPlayersRecordDetail.PlayerPoint1, fourPlayersRecordDetail.PlayerPoint2, fourPlayersRecordDetail.PlayerPoint3, fourPlayersRecordDetail.PlayerPoint4);
                PlayerWinds playerWinds = new PlayerWinds((Winds)fourPlayersRecordDetail.PlayerWind1, (Winds)fourPlayersRecordDetail.PlayerWind2, (Winds)fourPlayersRecordDetail.PlayerWind3, (Winds)fourPlayersRecordDetail.PlayerWind4);
                AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, fourPlayersBonus);
                AdjustmentScoreLabel1.BindingContext = adjustmentPoints.AdjustmentScore1;
                AdjustmentScoreLabel2.BindingContext = adjustmentPoints.AdjustmentScore2;
                AdjustmentScoreLabel3.BindingContext = adjustmentPoints.AdjustmentScore3;
                AdjustmentScoreLabel4.BindingContext = adjustmentPoints.AdjustmentScore4;
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

                if (_PlayerPointEntries.All(entry => !string.IsNullOrEmpty(entry.Text))) {
                    if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == _WindPickers.Count()) {
                        using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                            ShowAdjustmentScore(db);
                        }
                        return;
                    }
                }
            } else {
                playerPointEntry.Text = string.Empty;
            }
            _AdjustmentLabels.ForEach(label => label.BindingContext = null);
        }
        private async void WindPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (_PlayerPointEntries.All(entry => !string.IsNullOrEmpty(entry.Text))) {
                if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == _WindPickers.Count()) {
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
                int bonusID = (int)Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()];
                FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                if (int.Parse(PlayerPointEntry1.Text) + int.Parse(PlayerPointEntry2.Text) + int.Parse(PlayerPointEntry3.Text) + int.Parse(PlayerPointEntry4.Text) != fourPlayersBonus.OriginPoint * 4) {
                    if (!await DisplayAlert("得点確認", "点数の合計が原点と等しくありませんがこのまま登録しますか？", "Yes", "No")) {
                        return;
                    }
                }
                FourPlayersRecordDetail fourPlayersRecordDetail = db.Table<FourPlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID);
                db.Update(new FourPlayersRecordDetail() {
                    RecordDetailID = _RecordDetailID,
                    RecordID = fourPlayersRecordDetail.RecordID,
                    PlayerPoint1 = int.Parse(PlayerPointEntry1.Text),
                    PlayerPoint2 = int.Parse(PlayerPointEntry2.Text),
                    PlayerPoint3 = int.Parse(PlayerPointEntry3.Text),
                    PlayerPoint4 = int.Parse(PlayerPointEntry4.Text),
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