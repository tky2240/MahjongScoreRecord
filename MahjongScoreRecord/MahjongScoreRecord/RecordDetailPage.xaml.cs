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
        private readonly RecordListItem _RecordListItem;
        public RecordDetailPage(RecordListItem record) {
            InitializeComponent();
            _RecordListItem = record;
            _RecordID = _RecordListItem.RecordID;
            _PlayerNames = new PlayerNames(_RecordListItem.PlayerName1, _RecordListItem.PlayerName2, _RecordListItem.PlayerName3, _RecordListItem.PlayerName4);
            RecordNameLabel.BindingContext = _RecordListItem.RecordName;
            RecordTimeLabel.BindingContext = _RecordListItem.RecordTime.ToString();
        }

        private async void RecordDetailPage_Appearing(object sender, EventArgs e) {
            SQLiteConnection db = await DBOperations.ConnectDB();
            List<FourPlayersRecordDetail> fourPlayersRecordDetails = db.Table<FourPlayersRecordDetail>().Where(detail => detail.RecordID == _RecordID).ToList();
            List<Player> players = db.Table<Player>().ToList();
            FourPlayersRecord fourPlayersRecord = db.Table<FourPlayersRecord>().First(record => record.RecordID == _RecordID);
            db.Dispose();
            List<RecordDetailListViewItem> recordDetailListViewItems = new List<RecordDetailListViewItem>();
            fourPlayersRecordDetails.ForEach(detail => {
                PlayerPoints playerPoints = new PlayerPoints(detail.PlayerPoint1, detail.PlayerPoint2, detail.PlayerPoint3, detail.PlayerPoint4);
                recordDetailListViewItems.Add(new RecordDetailListViewItem(detail.RecordDetailID, _PlayerNames, playerPoints, 
                                              new AdjustmentPoints(playerPoints, new PlayerWinds((Winds)detail.PlayerWind1, (Winds)detail.PlayerWind2, (Winds)detail.PlayerWind3, (Winds)detail.PlayerWind4)),
                                              detail.MatchCount));
            });
            RecordDetailListView.ItemsSource = recordDetailListViewItems;
            db.Dispose();
        }

        private async void RegisterRecordDetailButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new RecordDetailRegisterPage(_RecordListItem)), true);
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }

    public class RecordDetailListViewItem {
        public RecordDetailListViewItem(int recordDetailID, PlayerNames playerNames, PlayerPoints playerPoints, AdjustmentPoints adjustmentPoints, int matchCount) {
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

    }

}