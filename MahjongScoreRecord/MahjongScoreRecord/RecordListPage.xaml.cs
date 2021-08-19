using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordListPage : ContentPage {
        public RecordListPage() {
            InitializeComponent();
            List<RecordListItem> recordListItems = new List<RecordListItem>();
            for(int i = 0; i < 50; i++) {
                recordListItems.Add(new RecordListItem(i.ToString(), $"hoge{i}"));
            }
            RecordListView.ItemsSource = recordListItems;
        }

        private void RecordListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            ListView listView = (ListView)sender;
            if(listView.SelectedItem == null) {
                return;
            }
            RecordListItem selectedItem = (RecordListItem)listView.SelectedItem;
            DisplayAlert("選択", selectedItem.RecordName, "OK");
            listView.SelectedItem = null;
            Navigation.PushModalAsync(new NavigationPage(new RecordDetailPage()));
        }
    }
    public class RecordListItem {
        public RecordListItem(string recordName, string players) {
            RecordName = recordName;
            Players = players;
        }
        public string RecordName { get; }
        public string Players { get; }
    }
}