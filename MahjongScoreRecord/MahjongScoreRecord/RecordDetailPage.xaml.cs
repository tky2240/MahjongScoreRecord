using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MahjongScoreRecord.Models;
using SQLite;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordDetailPage : ContentPage {
        private readonly int _RecordID;
        private readonly PlayerNames _PlayerNames;
        public RecordDetailPage(RecordListItem record) {
            _RecordID = record.RecordID;
            _PlayerNames = new PlayerNames(record.PlayerName1, record.PlayerName2, record.PlayerName3, record.PlayerName4);
        }

        private async void RecordDetailPage_Appearing(object sender, EventArgs e) {
            RecordNameLabel.Text = "hoge";
            RecordTimeLabel.Text = "fuga";
            SQLiteConnection db = await DBOperations.ConnectDB();
            /*List<FourPlayersRecordDetail> fourPlayersRecordDetails = db.Table<FourPlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).ToList();
            List<Player> players = db.Table<Player>().ToList();
            FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == _RecordID);
            db.Dispose();
            List<RecordDetailListViewItem> recordDetailListViewItems = new List<RecordDetailListViewItem>();
            fourPlayersRecordDetails.ForEach(detail => {
                PlayerPoints playerPoints = new PlayerPoints(detail.PlayerPoint1, detail.PlayerPoint2, detail.PlayerPoint3, detail.PlayerPoint4);
                recordDetailListViewItems.Add(new RecordDetailListViewItem(_PlayerNames, playerPoints, new AdjustmentPoints(playerPoints), detail.MatchCount));
            });*/
            //RecordDetailListView.ItemsSource = recordDetailListViewItems;
            db.Dispose();
        }
    }
    public class PlayerNames {
        public PlayerNames(string playerName1, string playerName2, string playerName3, string playerName4) {
            PlayerName1 = playerName1;
            PlayerName2 = playerName2;
            PlayerName3 = playerName3;
            PlayerName4 = playerName4;
        }
        public string PlayerName1 { get; }
        public string PlayerName2 { get; }
        public string PlayerName3 { get; }
        public string PlayerName4 { get; }
    }
    public class PlayerPoints {
        public PlayerPoints(int playerPoint1, int playerPoint2, int playerPoint3, int playerPoint4) {
            PlayerPoint1 = playerPoint1;
            PlayerPoint2 = playerPoint2;
            PlayerPoint3 = playerPoint3;
            PlayerPoint4 = playerPoint4;
        }
        public int PlayerPoint1 { get; }
        public int PlayerPoint2 { get; }
        public int PlayerPoint3 { get; }
        public int PlayerPoint4 { get; }
    }
    public class AdjustmentPoints {
        private readonly int Bonus1 = 20000;
        private readonly int Bonus2 = 10000;
        private readonly int Bonus3 = -10000;
        private readonly int Bonus4 = -20000;
        private readonly int TopPrize = 20000;
        public AdjustmentPoints(PlayerPoints playerPoints) {
            List<(int player, int point)> pointAndPlayers = new List<(int, int)>() { ( 1, playerPoints.PlayerPoint1 ),
                                                                                     ( 2, playerPoints.PlayerPoint2 ),
                                                                                     ( 3, playerPoints.PlayerPoint3 ),
                                                                                     ( 4, playerPoints.PlayerPoint4 )};
            List<(int player, int point)> sortedPointAndPlayers = pointAndPlayers.OrderByDescending(pointAndPlayer => pointAndPlayer.point).ToList();
            sortedPointAndPlayers[0] = (sortedPointAndPlayers[0].player, sortedPointAndPlayers[0].point + Bonus1 + TopPrize);
            sortedPointAndPlayers[1] = (sortedPointAndPlayers[1].player, sortedPointAndPlayers[1].point + Bonus2);
            sortedPointAndPlayers[2] = (sortedPointAndPlayers[2].player, sortedPointAndPlayers[2].point + Bonus3);
            sortedPointAndPlayers[3] = (sortedPointAndPlayers[3].player, sortedPointAndPlayers[3].point + Bonus2);

            AdjustmentPoint1 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 1).point;
            AdjustmentPoint2 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 2).point;
            AdjustmentPoint3 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 3).point;
            AdjustmentPoint4 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 4).point;
        }
        public double AdjustmentPoint1 { get; }
        public double AdjustmentPoint2 { get; }
        public double AdjustmentPoint3 { get; }
        public double AdjustmentPoint4 { get; }
    }
    public class RecordDetailListViewItem {
        public RecordDetailListViewItem(PlayerNames playerNames, PlayerPoints playerPoints, AdjustmentPoints adjustmentPoints, int matchCount) {
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
            MatchCount = matchCount;

        }
        public string PlayerName1 { get; }
        public string PlayerName2 { get; }
        public string PlayerName3 { get; }
        public string PlayerName4 { get; }
        public int PlayerPoint1 { get; }
        public int PlayerPoint2 { get; }
        public int PlayerPoint3 { get; }
        public int PlayerPoint4 { get; }
        public double AdjustmentPoint1 { get; }
        public double AdjustmentPoint2 { get; }
        public double AdjustmentPoint3 { get; }
        public double AdjustmentPoint4 { get; }
        public int MatchCount { get; }

    }

}