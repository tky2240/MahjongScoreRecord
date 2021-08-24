using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using MahjongScoreRecord.Models;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordListPage : ContentPage {
        public RecordListPage() {
            InitializeComponent();
        }

        private async void RecordListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            ListView listView = (ListView)sender;
            if(listView.SelectedItem == null) {
                return;
            }
            RecordListItem selectedItem = (RecordListItem)listView.SelectedItem;
            await DisplayAlert("選択", selectedItem.RecordName, "OK");
            listView.SelectedItem = null;
            await Navigation.PushModalAsync(new NavigationPage(new RecordDetailListPage(selectedItem)));
        }

        private async void RecordListPage_Appearing(object sender, EventArgs e) {
            SQLiteConnection db = await DBOperations.ConnectDB();
            List<FourPlayersRecord> fourPlayersRecords = db.Table<FourPlayersRecord>().ToList();
            List<Player> players = db.Table<Player>().ToList();
            List<RecordListItem> recordListItems = new List<RecordListItem>();
            fourPlayersRecords.ForEach(record => {
                recordListItems.Add(new RecordListItem( record.RecordID,
                                                        record.RecordName,
                                                        players.First(player => player.PlayerID == record.PlayerID1).PlayerName,
                                                        players.First(player => player.PlayerID == record.PlayerID2).PlayerName,
                                                        players.First(player => player.PlayerID == record.PlayerID3).PlayerName,
                                                        players.First(player => player.PlayerID == record.PlayerID4).PlayerName,
                                                        record.RecordTime));
            });
            RecordListView.ItemsSource = recordListItems;
        }

        private async void RegisterRecordButton_Clicked(object sender, EventArgs e) {
            SQLiteConnection db = await DBOperations.ConnectDB();
            List<Player> players = db.Table<Player>().ToList();
            db.Dispose();
            await Navigation.PushModalAsync(new NavigationPage(new RecordRegisterPage(players)),true);
        }

        private void BackButton_Clicked(object sender, EventArgs e) {

        }
    }
    public class RecordListItem {
        public RecordListItem(int recordID, string recordName, string playerName1, string playerName2, string playerName3, string playerName4, DateTime recordTime) {
            RecordID = recordID;
            RecordName = recordName;
            PlayerName1 = playerName1;
            PlayerName2 = playerName2;
            PlayerName3 = playerName3;
            PlayerName4 = playerName4;
            RecordTime = recordTime;
        }
        public int RecordID { get; }
        public string RecordName { get; }
        public string PlayerName1 { get; }
        public string PlayerName2 { get; }
        public string PlayerName3 { get; }
        public string PlayerName4 { get; }
        public DateTime RecordTime { get; }
    }
}