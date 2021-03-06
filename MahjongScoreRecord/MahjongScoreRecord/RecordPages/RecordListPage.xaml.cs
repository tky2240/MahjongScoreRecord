using MahjongScoreRecord.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordListPage : ContentPage {
        public RecordListPage() {
            InitializeComponent();
        }
        private async void RecordListPage_Appearing(object sender, EventArgs e) {
            List<RecordListItem> recordListItems = new List<RecordListItem>();
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                List<Player> players = db.Table<Player>().ToList();
                if (Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                    List<FourPlayersRecord> fourPlayersRecords = db.Table<FourPlayersRecord>().ToList();
                    fourPlayersRecords.ForEach(record => recordListItems.Add(new RecordListItem(record, players)));
                    FourPlayersModeRadioButton.IsChecked = true;
                } else if (Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                    List<ThreePlayersRecord> threePlayersRecords = db.Table<ThreePlayersRecord>().ToList();
                    threePlayersRecords.ForEach(record => recordListItems.Add(new RecordListItem(record, players)));
                    ThreePlayersModeRadioButton.IsChecked = true;
                }
            }
            RecordListView.ItemsSource = recordListItems;
        }
        private async void RecordListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            ListView recordListView = (ListView)sender;
            RecordListItem selectedRecord = (RecordListItem)recordListView.SelectedItem;
            if (selectedRecord == null) {
                return;
            }
            recordListView.SelectedItem = null;
            await Navigation.PushModalAsync(new NavigationPage(new RecordDetailListPage(selectedRecord.RecordID)));
        }
        private async void PlayersModeRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e) {
            if (FourPlayersModeRadioButton.IsChecked) {
                await Globals.SetCurrentPlayersModeAsync(PlayersMode.Four);
            } else if (ThreePlayersModeRadioButton.IsChecked) {
                await Globals.SetCurrentPlayersModeAsync(PlayersMode.Three);
            } else {
                await DisplayAlert("エラー", "エラーが発生しました\n四麻モードに設定します", "OK");
                await Globals.SetCurrentPlayersModeAsync(PlayersMode.Four);
            }
            RecordListPage_Appearing(null, null);
        }
        private async void RegisterRecordButton_Clicked(object sender, EventArgs e) {
            List<Player> players = new List<Player>();
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                players = db.Table<Player>().ToList();
            }
            await Navigation.PushModalAsync(new NavigationPage(new RecordRegisterPage(players)), true);
        }
        private class RecordListItem {
            public RecordListItem(FourPlayersRecord fourPlayersRecord, List<Player> players) {
                RecordID = fourPlayersRecord.RecordID;
                RecordName = fourPlayersRecord.RecordName;
                PlayerName1 = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID1).PlayerName;
                PlayerName2 = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID2).PlayerName;
                PlayerName3 = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID3).PlayerName;
                PlayerName4 = players.First(player => player.PlayerID == fourPlayersRecord.PlayerID4).PlayerName;
                RecordTime = fourPlayersRecord.RecordTime;
                PlayersMode = PlayersMode.Four;
            }
            public RecordListItem(ThreePlayersRecord threePlayersRecord, List<Player> players) {
                RecordID = threePlayersRecord.RecordID;
                RecordName = threePlayersRecord.RecordName;
                PlayerName1 = players.First(player => player.PlayerID == threePlayersRecord.PlayerID1).PlayerName;
                PlayerName2 = players.First(player => player.PlayerID == threePlayersRecord.PlayerID2).PlayerName;
                PlayerName3 = players.First(player => player.PlayerID == threePlayersRecord.PlayerID3).PlayerName;
                PlayerName4 = null;
                RecordTime = threePlayersRecord.RecordTime;
                PlayersMode = PlayersMode.Three;
            }
            public int RecordID { get; }
            public string RecordName { get; }
            public string PlayerName1 { get; }
            public string PlayerName2 { get; }
            public string PlayerName3 { get; }
            public string PlayerName4 { get; }
            public DateTime RecordTime { get; }
            public PlayersMode PlayersMode { get; }
        }
    }
}