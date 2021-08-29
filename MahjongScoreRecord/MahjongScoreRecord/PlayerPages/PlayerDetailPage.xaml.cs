using MahjongScoreRecord.Models;
using Microcharts;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerDetailPage : ContentPage {
        private readonly int _PlayerID;
        public PlayerDetailPage(int playerID) {
            InitializeComponent();
            _PlayerID = playerID;
        }

        private async void PlayerDetailPage_Appearing(object sender, EventArgs e) {
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                List<FourPlayersRecord> fourPlayersRecords = db.Table<FourPlayersRecord>().Where(detail => detail.PlayerID1 == _PlayerID ||
                                                                                                           detail.PlayerID2 == _PlayerID ||
                                                                                                           detail.PlayerID3 == _PlayerID ||
                                                                                                           detail.PlayerID4 == _PlayerID).ToList();
                List<int> recordIDs = fourPlayersRecords.Select(record => record.RecordID).ToList();
                List<FourPlayersRecordDetail> fourPlayersRecordDetails = db.Table<FourPlayersRecordDetail>().Where(detail => recordIDs.Contains(detail.RecordID)).ToList();
                int bonusID = Globals.GetCurrentFourPlayersBonusID();
                FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == bonusID);
                List<ChartEntry> chartEntries = new List<ChartEntry>();
                double totalSocre = 0;
                foreach (FourPlayersRecord fourPlayersRecord in fourPlayersRecords) {
                    ReadOnlyCollection<int> playerIDs = new ReadOnlyCollection<int>(new List<int>() { fourPlayersRecord.PlayerID1, fourPlayersRecord.PlayerID2, fourPlayersRecord.PlayerID3, fourPlayersRecord.PlayerID4 });
                    int playerIndex = playerIDs.IndexOf(_PlayerID);
                    foreach (FourPlayersRecordDetail fourPlayersRecordDetail in fourPlayersRecordDetails) {
                        PlayerPoints playerPoints = new PlayerPoints(fourPlayersRecordDetail.PlayerPoint1, fourPlayersRecordDetail.PlayerPoint2, fourPlayersRecordDetail.PlayerPoint3, fourPlayersRecordDetail.PlayerPoint4);
                        PlayerWinds playerWinds = new PlayerWinds((Winds)fourPlayersRecordDetail.PlayerWind1, (Winds)fourPlayersRecordDetail.PlayerWind2, (Winds)fourPlayersRecordDetail.PlayerWind3, (Winds)fourPlayersRecordDetail.PlayerWind4);
                        AdjustmentPoints adjustmentPoints = new AdjustmentPoints(playerPoints, playerWinds, fourPlayersBonus);
                        List<double> adjustmentScores = new List<double>() { adjustmentPoints.AdjustmentScore1, adjustmentPoints.AdjustmentScore2, adjustmentPoints.AdjustmentScore2, adjustmentPoints.AdjustmentScore4 };
                        totalSocre += adjustmentScores[playerIndex];
                        chartEntries.Add(new ChartEntry(
                            (float)totalSocre) {
                                Label = $"{fourPlayersRecord.RecordName}:{fourPlayersRecord.RecordTime}\n{fourPlayersRecordDetail.MatchCount}局目",
                                ValueLabel = $"{totalSocre}"
                            }
                        );
                    }
                }
                PlayerDetailChartView.Chart = new LineChart() {
                    Entries = chartEntries,
                    AnimationDuration = TimeSpan.FromMilliseconds(500),
                    IsAnimated = true,
                    LineMode = LineMode.Straight,
                    PointMode = PointMode.Circle,
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal
                };
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}