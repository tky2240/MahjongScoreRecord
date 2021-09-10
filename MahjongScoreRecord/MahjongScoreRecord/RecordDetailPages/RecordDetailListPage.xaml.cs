using MahjongScoreRecord.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordDetailListPage : ContentPage {
        private readonly int _RecordID;
        public RecordDetailListPage(int recordID) {
            InitializeComponent();
            _RecordID = recordID;
            EditButton.BindingContext = ImageSource.FromResource("MahjongScoreRecord.Images.Edit.png");
            DeleteButton.BindingContext = ImageSource.FromResource("MahjongScoreRecord.Images.Delete.png");
        }

        private async void RecordDetailPage_Appearing(object sender, EventArgs e) {
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                List<RecordDetailListItem> recordDetailListViewItems = new List<RecordDetailListItem>();
                if (Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                    List<FourPlayersRecordDetail> fourPlayersRecordDetails = db.Table<FourPlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).ToList();
                    List<Player> players = db.Table<Player>().ToList();
                    FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == _RecordID);
                    int bonusID = Globals.GetCurrentFourPlayersBonusID();
                    FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                    RecordNameLabel.BindingContext = fourPlayersRecord.RecordName;
                    RecordTimeLabel.BindingContext = fourPlayersRecord.RecordTime.ToString();
                    PlayerNames playerNames = new PlayerNames(players.First(player => player.PlayerID == fourPlayersRecord.PlayerID1).PlayerName,
                                                              players.First(player => player.PlayerID == fourPlayersRecord.PlayerID2).PlayerName,
                                                              players.First(player => player.PlayerID == fourPlayersRecord.PlayerID3).PlayerName,
                                                              players.First(player => player.PlayerID == fourPlayersRecord.PlayerID4).PlayerName);
                    fourPlayersRecordDetails.ForEach(detail => {
                        PlayerPoints playerPoints = new PlayerPoints(detail.PlayerPoint1, detail.PlayerPoint2, detail.PlayerPoint3, detail.PlayerPoint4);
                        recordDetailListViewItems.Add(new RecordDetailListItem(detail.RecordDetailID, playerNames, playerPoints,
                                                      new AdjustmentPoints(playerPoints, new PlayerWinds((Winds)detail.PlayerWind1, (Winds)detail.PlayerWind2, (Winds)detail.PlayerWind3, (Winds)detail.PlayerWind4), fourPlayersBonus),
                                                      detail.MatchCount));
                    });
                }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                    List<ThreePlayersRecordDetail> threePlayersRecordDetails = db.Table<ThreePlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).ToList();
                    List<Player> players = db.Table<Player>().ToList();
                    ThreePlayersRecord threePlayersRecord = db.Table<ThreePlayersRecord>().First(record => record.RecordID == _RecordID);
                    int bonusID = Globals.GetCurrentThreePlayersBonusID();
                    ThreePlayersBonus threePlayersBonus = db.Table<ThreePlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                    RecordNameLabel.BindingContext = threePlayersRecord.RecordName;
                    RecordTimeLabel.BindingContext = threePlayersRecord.RecordTime.ToString();
                    PlayerNames playerNames = new PlayerNames(players.First(player => player.PlayerID == threePlayersRecord.PlayerID1).PlayerName,
                                                              players.First(player => player.PlayerID == threePlayersRecord.PlayerID2).PlayerName,
                                                              players.First(player => player.PlayerID == threePlayersRecord.PlayerID3).PlayerName);
                    threePlayersRecordDetails.ForEach(detail => {
                        PlayerPoints playerPoints = new PlayerPoints(detail.PlayerPoint1, detail.PlayerPoint2, detail.PlayerPoint3);
                        recordDetailListViewItems.Add(new RecordDetailListItem(detail.RecordDetailID, playerNames, playerPoints,
                                                      new AdjustmentPoints(playerPoints, new PlayerWinds((Winds)detail.PlayerWind1, (Winds)detail.PlayerWind2, (Winds)detail.PlayerWind3), threePlayersBonus),
                                                      detail.MatchCount));
                    });
                }
                RecordDetailListView.ItemsSource = recordDetailListViewItems;
            }
        }

        private async void RegisterRecordDetailButton_Clicked(object sender, EventArgs e) {
            await Navigation.PushModalAsync(new NavigationPage(new RecordDetailRegisterPage(_RecordID)), true);
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }

        private async void RecordDetailListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            ListView recordDetailListView = (ListView)sender;
            RecordDetailListItem selectedDetail = (RecordDetailListItem)recordDetailListView.SelectedItem;
            if (selectedDetail == null) {
                return;
            }
            await Navigation.PushModalAsync(new NavigationPage(new RecordDetailUpdatePage(selectedDetail.RecordDetailID)), true);
        }

        private async void EditButton_Clicked(object sender, EventArgs e) {
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                List<Player> players = db.Table<Player>().ToList();
                if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                    FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == _RecordID);
                    await Navigation.PushModalAsync(new NavigationPage(new RecordUpdatePage(players, fourPlayersRecord)), true);
                }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                    ThreePlayersRecord threePlayersRecord = db.Table<ThreePlayersRecord>().First(record => record.RecordID == _RecordID);
                    await Navigation.PushModalAsync(new NavigationPage(new RecordUpdatePage(players, threePlayersRecord)), true);
                }
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e) {
            if (await DisplayAlert("削除確認", $"対局名「{RecordNameLabel.Text}」\n記録日「{RecordTimeLabel.Text}」\n削除してもよろしいですか？", "Yes", "No")) {
                using(SQLiteConnection db = await DBOperations.ConnectDB()) {
                    if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                        db.Table<FourPlayersRecordDetail>().Delete(detail => detail.RecordID == _RecordID);
                        db.Table<FourPlayersRecord>().Delete(record => record.RecordID == _RecordID);
                    }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                        db.Table<ThreePlayersRecordDetail>().Delete(detail => detail.RecordID == _RecordID);
                        db.Table<ThreePlayersRecord>().Delete(record => record.RecordID == _RecordID);
                    }
                }
                await DisplayAlert("削除完了", "削除が完了しました", "OK");
                await Navigation.PopModalAsync(true);
            }
        }
        private class RecordDetailListItem {
            public RecordDetailListItem(int recordDetailID, PlayerNames playerNames, PlayerPoints playerPoints, AdjustmentPoints adjustmentPoints, int matchCount) {
                RecordDetailID = recordDetailID;
                PlayerName1 = playerNames.PlayerName1;
                PlayerName2 = playerNames.PlayerName2;
                PlayerName3 = playerNames.PlayerName3;
                PlayerName4 = playerNames.PlayerName4;
                PlayerPoint1 = playerPoints.PlayerPoint1;
                PlayerPoint2 = playerPoints.PlayerPoint2;
                PlayerPoint3 = playerPoints.PlayerPoint3;
                PlayerPoint4 = playerPoints.PlayerPoint4;
                AdjustmentPoint1 = adjustmentPoints.AdjustmentPoint1;
                AdjustmentPoint2 = adjustmentPoints.AdjustmentPoint2;
                AdjustmentPoint3 = adjustmentPoints.AdjustmentPoint3;
                AdjustmentPoint4 = adjustmentPoints.AdjustmentPoint4;
                AdjustmentScore1 = adjustmentPoints.AdjustmentScore1;
                AdjustmentScore2 = adjustmentPoints.AdjustmentScore2;
                AdjustmentScore3 = adjustmentPoints.AdjustmentScore3;
                AdjustmentScore4 = adjustmentPoints.AdjustmentScore4;
                MatchCount = matchCount;
                MatchCountText = $"{matchCount}対局目";
                PlayersMode = Globals.GetCurrentPlayersMode();
            }
            public int RecordDetailID { get; }
            public string PlayerName1 { get; }
            public string PlayerName2 { get; }
            public string PlayerName3 { get; }
            public string PlayerName4 { get; }
            public int PlayerPoint1 { get; }
            public int PlayerPoint2 { get; }
            public int PlayerPoint3 { get; }
            public int PlayerPoint4 { get; }
            public int AdjustmentPoint1 { get; }
            public int AdjustmentPoint2 { get; }
            public int AdjustmentPoint3 { get; }
            public int AdjustmentPoint4 { get; }
            public double AdjustmentScore1 { get; }
            public double AdjustmentScore2 { get; }
            public double AdjustmentScore3 { get; }
            public double AdjustmentScore4 { get; }
            public int MatchCount { get; }
            public string MatchCountText { get; }
            public PlayersMode PlayersMode { get; }
        }
    }
}