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
        //private readonly RecordListItem _RecordListItem;
        private readonly List<Entry> _PlayerPointEntries;
        private readonly List<Picker> _WindPickers;
        private readonly int _RecordID;
        public RecordDetailRegisterPage(int recordID) {
            InitializeComponent();
            _RecordID = recordID;
            _PlayerPointEntries = new List<Entry>() { PlayerPoint1Entry, PlayerPoint2Entry, PlayerPoint3Entry, PlayerPoint4Entry };
            _WindPickers = new List<Picker>() { WindPicker1, WindPicker2, WindPicker3, WindPicker4 };
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
                PlayerName1Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID1).PlayerName;
                PlayerName2Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID2).PlayerName;
                PlayerName3Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID3).PlayerName;
                PlayerName4Label.BindingContext = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID4).PlayerName;
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

        private async void PlayerPointEntry_Unfocused(object sender, FocusEventArgs e) {
            Entry playerPointEntry = (Entry)sender;
            if (int.TryParse(playerPointEntry.Text, out int point)) {
                playerPointEntry.Text = point.ToString();

                if (_PlayerPointEntries.All(entry => !string.IsNullOrEmpty(entry.Text))) {
                    if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == 4) {
                        using(SQLiteConnection db = await DBOperations.ConnectDB()) {
                            int bonusID = (int)Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()];
                            FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                            PlayerPoints playerPoints = new PlayerPoints(int.Parse(PlayerPoint1Entry.Text),
                                                                 int.Parse(PlayerPoint2Entry.Text),
                                                                 int.Parse(PlayerPoint3Entry.Text),
                                                                 int.Parse(PlayerPoint4Entry.Text));
                            PlayerWinds playerWinds = new PlayerWinds(((KeyValuePair<Winds, string>)WindPicker1.SelectedItem).Key,
                                                                      ((KeyValuePair<Winds, string>)WindPicker2.SelectedItem).Key,
                                                                      ((KeyValuePair<Winds, string>)WindPicker3.SelectedItem).Key,
                                                                      ((KeyValuePair<Winds, string>)WindPicker4.SelectedItem).Key);
                            AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, fourPlayersBonus);
                            AdjustmentScore1Label.BindingContext = adjustmentPoints.AdjustmentScore1;
                            AdjustmentScore2Label.BindingContext = adjustmentPoints.AdjustmentScore2;
                            AdjustmentScore3Label.BindingContext = adjustmentPoints.AdjustmentScore3;
                            AdjustmentScore4Label.BindingContext = adjustmentPoints.AdjustmentScore4;
                        }
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
        private async void WindPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (_PlayerPointEntries.All(entry => !string.IsNullOrEmpty(entry.Text))) {
                if (_WindPickers.Select(picker => ((KeyValuePair<Winds, string>)picker.SelectedItem).Key).Distinct().Count() == 4) {
                    using(SQLiteConnection db = await DBOperations.ConnectDB()) {
                        int bonusID = (int)Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()];
                        FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                        PlayerPoints playerPoints = new PlayerPoints(int.Parse(PlayerPoint1Entry.Text),
                                                             int.Parse(PlayerPoint2Entry.Text),
                                                             int.Parse(PlayerPoint3Entry.Text),
                                                             int.Parse(PlayerPoint4Entry.Text));
                        PlayerWinds playerWinds = new PlayerWinds(((KeyValuePair<Winds, string>)WindPicker1.SelectedItem).Key,
                                                                  ((KeyValuePair<Winds, string>)WindPicker2.SelectedItem).Key,
                                                                  ((KeyValuePair<Winds, string>)WindPicker3.SelectedItem).Key,
                                                                  ((KeyValuePair<Winds, string>)WindPicker4.SelectedItem).Key);
                        AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, fourPlayersBonus);
                        AdjustmentScore1Label.BindingContext = adjustmentPoints.AdjustmentScore1;
                        AdjustmentScore2Label.BindingContext = adjustmentPoints.AdjustmentScore2;
                        AdjustmentScore3Label.BindingContext = adjustmentPoints.AdjustmentScore3;
                        AdjustmentScore4Label.BindingContext = adjustmentPoints.AdjustmentScore4;
                    }
                    return;
                }
            }
            AdjustmentScore1Label.BindingContext = null;
            AdjustmentScore2Label.BindingContext = null;
            AdjustmentScore3Label.BindingContext = null;
            AdjustmentScore4Label.BindingContext = null;
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
                int matchCount = 0;
                if (db.Table<FourPlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).Any()) {
                    matchCount = db.Table<FourPlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).Select(detail => detail.MatchCount).Max();
                }
                db.Insert(new FourPlayersRecordDetail() {
                    RecordID = _RecordID,
                    PlayerPoint1 = int.Parse(PlayerPoint1Entry.Text),
                    PlayerPoint2 = int.Parse(PlayerPoint2Entry.Text),
                    PlayerPoint3 = int.Parse(PlayerPoint3Entry.Text),
                    PlayerPoint4 = int.Parse(PlayerPoint4Entry.Text),
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