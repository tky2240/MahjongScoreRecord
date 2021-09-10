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
            if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                _PlayerPointEntries = new List<Entry>() { PlayerPointEntry1, PlayerPointEntry2, PlayerPointEntry3, PlayerPointEntry4 };
                _WindPickers = new List<Picker>() { WindPicker1, WindPicker2, WindPicker3, WindPicker4 };
                _AdjustmentLabels = new List<Label>() { AdjustmentScoreLabel1, AdjustmentScoreLabel2, AdjustmentScoreLabel3, AdjustmentScoreLabel4 };
            }else if (Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                _PlayerPointEntries = new List<Entry>() { PlayerPointEntry1, PlayerPointEntry2, PlayerPointEntry3};
                _WindPickers = new List<Picker>() { WindPicker1, WindPicker2, WindPicker3};
                _AdjustmentLabels = new List<Label>() { AdjustmentScoreLabel1, AdjustmentScoreLabel2, AdjustmentScoreLabel3};
            }
        }
        private async void RecordDetailUpdatePage_Appearing(object sender, EventArgs e) {
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                    FourPlayersRecordDetail fourPlayersRecordDetail = db.Table<FourPlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID);
                    FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == fourPlayersRecordDetail.RecordID);
                    int bonusID = Globals.GetCurrentFourPlayersBonusID();
                    FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                    List<Player> players = db.Table<Player>().ToList();
                    PlayerNameLabel1.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID1).PlayerName;
                    PlayerNameLabel2.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID2).PlayerName;
                    PlayerNameLabel3.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID3).PlayerName;
                    PlayerNameLabel4.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID4).PlayerName;
                    _WindPickers.ForEach(picker => picker.ItemsSource = Globals.WindsAndTexts);
                    WindPicker1.SelectedItem = Globals.WindsAndTexts.First(wind => (int)wind.Wind == fourPlayersRecordDetail.PlayerWind1);
                    WindPicker2.SelectedItem = Globals.WindsAndTexts.First(wind => (int)wind.Wind == fourPlayersRecordDetail.PlayerWind2);
                    WindPicker3.SelectedItem = Globals.WindsAndTexts.First(wind => (int)wind.Wind == fourPlayersRecordDetail.PlayerWind3);
                    WindPicker4.SelectedItem = Globals.WindsAndTexts.First(wind => (int)wind.Wind == fourPlayersRecordDetail.PlayerWind4);
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
                }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                    ThreePlayersRecordDetail threePlayersRecordDetail = db.Table<ThreePlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID);
                    ThreePlayersRecord threePlayersRecord = db.Table<ThreePlayersRecord>().First(record => record.RecordID == threePlayersRecordDetail.RecordID);
                    int bonusID = Globals.GetCurrentThreePlayersBonusID();
                    ThreePlayersBonus threePlayersBonus = db.Table<ThreePlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                    List<Player> players = db.Table<Player>().ToList();
                    PlayerNameLabel1.BindingContext = players.First(player => player.PlayerID == threePlayersRecord.PlayerID1).PlayerName;
                    PlayerNameLabel2.BindingContext = players.First(player => player.PlayerID == threePlayersRecord.PlayerID2).PlayerName;
                    PlayerNameLabel3.BindingContext = players.First(player => player.PlayerID == threePlayersRecord.PlayerID3).PlayerName;
                    _WindPickers.ForEach(picker => picker.ItemsSource = Globals.WindsAndTexts.Where(wind => wind.Wind != Winds.North).ToList());
                    WindPicker1.SelectedItem = Globals.WindsAndTexts.First(wind => (int)wind.Wind == threePlayersRecordDetail.PlayerWind1);
                    WindPicker2.SelectedItem = Globals.WindsAndTexts.First(wind => (int)wind.Wind == threePlayersRecordDetail.PlayerWind2);
                    WindPicker3.SelectedItem = Globals.WindsAndTexts.First(wind => (int)wind.Wind == threePlayersRecordDetail.PlayerWind3);
                    PlayerPointEntry1.Text = threePlayersRecordDetail.PlayerPoint1.ToString();
                    PlayerPointEntry2.Text = threePlayersRecordDetail.PlayerPoint2.ToString();
                    PlayerPointEntry3.Text = threePlayersRecordDetail.PlayerPoint3.ToString();
                    PlayerPoints playerPoints = new PlayerPoints(threePlayersRecordDetail.PlayerPoint1, threePlayersRecordDetail.PlayerPoint2, threePlayersRecordDetail.PlayerPoint3);
                    PlayerWinds playerWinds = new PlayerWinds((Winds)threePlayersRecordDetail.PlayerWind1, (Winds)threePlayersRecordDetail.PlayerWind2, (Winds)threePlayersRecordDetail.PlayerWind3);
                    AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, threePlayersBonus);
                    AdjustmentScoreLabel1.BindingContext = adjustmentPoints.AdjustmentScore1;
                    AdjustmentScoreLabel2.BindingContext = adjustmentPoints.AdjustmentScore2;
                    AdjustmentScoreLabel3.BindingContext = adjustmentPoints.AdjustmentScore3;
                }
                PlayerStackLayout4.BindingContext = Globals.GetCurrentPlayersMode();
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
                    if (_WindPickers.Select(picker => ((Globals.WindAndText)picker.SelectedItem).Wind).Distinct().Count() == _WindPickers.Count()) {
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
                if (_WindPickers.Select(picker => ((Globals.WindAndText)picker.SelectedItem).Wind).Distinct().Count() == _WindPickers.Count()) {
                    using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                        ShowAdjustmentScore(db);
                    }
                    return;
                }
            }
            _AdjustmentLabels.ForEach(label => label.BindingContext = null);
        }
        private void ShowAdjustmentScore(SQLiteConnection db) {
            if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                int bonusID = Globals.GetCurrentFourPlayersBonusID();
                FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                PlayerPoints playerPoints = new PlayerPoints(int.Parse(PlayerPointEntry1.Text),
                                                             int.Parse(PlayerPointEntry2.Text),
                                                             int.Parse(PlayerPointEntry3.Text),
                                                             int.Parse(PlayerPointEntry4.Text));
                PlayerWinds playerWinds = new PlayerWinds(((Globals.WindAndText)WindPicker1.SelectedItem).Wind,
                                                          ((Globals.WindAndText)WindPicker2.SelectedItem).Wind,
                                                          ((Globals.WindAndText)WindPicker3.SelectedItem).Wind,
                                                          ((Globals.WindAndText)WindPicker4.SelectedItem).Wind);
                AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, fourPlayersBonus);
                AdjustmentScoreLabel1.BindingContext = adjustmentPoints.AdjustmentScore1;
                AdjustmentScoreLabel2.BindingContext = adjustmentPoints.AdjustmentScore2;
                AdjustmentScoreLabel3.BindingContext = adjustmentPoints.AdjustmentScore3;
                AdjustmentScoreLabel4.BindingContext = adjustmentPoints.AdjustmentScore4;
            }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                int bonusID = Globals.GetCurrentThreePlayersBonusID();
                ThreePlayersBonus threePlayersBonus = db.Table<ThreePlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                PlayerPoints playerPoints = new PlayerPoints(int.Parse(PlayerPointEntry1.Text),
                                                             int.Parse(PlayerPointEntry2.Text),
                                                             int.Parse(PlayerPointEntry3.Text));
                PlayerWinds playerWinds = new PlayerWinds(((Globals.WindAndText)WindPicker1.SelectedItem).Wind,
                                                          ((Globals.WindAndText)WindPicker2.SelectedItem).Wind,
                                                          ((Globals.WindAndText)WindPicker3.SelectedItem).Wind);
                AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, threePlayersBonus);
                AdjustmentScoreLabel1.BindingContext = adjustmentPoints.AdjustmentScore1;
                AdjustmentScoreLabel2.BindingContext = adjustmentPoints.AdjustmentScore2;
                AdjustmentScoreLabel3.BindingContext = adjustmentPoints.AdjustmentScore3;
            }
            
        }
        private async void UpdateButton_Clicked(object sender, EventArgs e) {
            if (_WindPickers.Select(picker => ((Globals.WindAndText)picker.SelectedItem).Wind).Distinct().Count() != _WindPickers.Count()) {
                await DisplayAlert("エラー", "風を正しく選択してください", "OK");
                return;
            }
            if (_PlayerPointEntries.Any(entry => string.IsNullOrEmpty(entry.Text))) {
                await DisplayAlert("エラー", "全得点を正しく入力してください", "OK");
                return;
            }
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                    int bonusID = Globals.GetCurrentFourPlayersBonusID();
                    FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                    if (int.Parse(PlayerPointEntry1.Text) + int.Parse(PlayerPointEntry2.Text) + int.Parse(PlayerPointEntry3.Text) + int.Parse(PlayerPointEntry4.Text) != fourPlayersBonus.OriginPoint * (int)PlayersMode.Four) {
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
                        PlayerWind1 = (int)((Globals.WindAndText)WindPicker1.SelectedItem).Wind,
                        PlayerWind2 = (int)((Globals.WindAndText)WindPicker2.SelectedItem).Wind,
                        PlayerWind3 = (int)((Globals.WindAndText)WindPicker3.SelectedItem).Wind,
                        PlayerWind4 = (int)((Globals.WindAndText)WindPicker4.SelectedItem).Wind,
                        MatchCount = fourPlayersRecordDetail.MatchCount
                    });
                }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                    int bonusID = Globals.GetCurrentThreePlayersBonusID();
                    ThreePlayersBonus threePlayersBonus = db.Table<ThreePlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                    if (int.Parse(PlayerPointEntry1.Text) + int.Parse(PlayerPointEntry2.Text) + int.Parse(PlayerPointEntry3.Text) != threePlayersBonus.OriginPoint * (int)PlayersMode.Three) {
                        if (!await DisplayAlert("得点確認", "点数の合計が原点と等しくありませんがこのまま登録しますか？", "Yes", "No")) {
                            return;
                        }
                    }
                    ThreePlayersRecordDetail threePlayersRecordDetail = db.Table<ThreePlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID);
                    db.Update(new ThreePlayersRecordDetail() {
                        RecordDetailID = _RecordDetailID,
                        RecordID = threePlayersRecordDetail.RecordID,
                        PlayerPoint1 = int.Parse(PlayerPointEntry1.Text),
                        PlayerPoint2 = int.Parse(PlayerPointEntry2.Text),
                        PlayerPoint3 = int.Parse(PlayerPointEntry3.Text),
                        PlayerWind1 = (int)((Globals.WindAndText)WindPicker1.SelectedItem).Wind,
                        PlayerWind2 = (int)((Globals.WindAndText)WindPicker2.SelectedItem).Wind,
                        PlayerWind3 = (int)((Globals.WindAndText)WindPicker3.SelectedItem).Wind,
                        MatchCount = threePlayersRecordDetail.MatchCount
                    });
                }
            }
            await Navigation.PopModalAsync(true);
        }
        private async void DeleteButton_Clicked(object sender, EventArgs e) {
            if (await DisplayAlert("削除確認", "この対局結果を削除しますか？", "Yes", "No")) {
                using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                    if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
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
                    }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                        int matchCount = db.Table<ThreePlayersRecordDetail>().First(detail => detail.RecordDetailID == _RecordDetailID).MatchCount;
                        db.Table<ThreePlayersRecordDetail>().Delete(detail => detail.RecordDetailID == _RecordDetailID);
                        db.UpdateAll(
                            db.Table<ThreePlayersRecordDetail>().Where(detail => detail.MatchCount > matchCount).Select(detail => new ThreePlayersRecordDetail() {
                                RecordDetailID = detail.RecordDetailID,
                                RecordID = detail.RecordID,
                                PlayerPoint1 = detail.PlayerPoint1,
                                PlayerPoint2 = detail.PlayerPoint2,
                                PlayerPoint3 = detail.PlayerPoint3,
                                PlayerWind1 = detail.PlayerWind1,
                                PlayerWind2 = detail.PlayerWind2,
                                PlayerWind3 = detail.PlayerWind3,
                                MatchCount = detail.MatchCount - 1
                            }).ToList()
                        );
                    }
                }
                await Navigation.PopModalAsync(true);
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}