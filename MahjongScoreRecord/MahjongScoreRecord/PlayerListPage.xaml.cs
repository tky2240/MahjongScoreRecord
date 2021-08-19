using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayerListPage : ContentPage {
        public PlayerListPage() {
            InitializeComponent();
            List<PlayerListViewItem> playerListViewItems = new List<PlayerListViewItem>();
            for(int i = 0; i < 50; i++) {
                playerListViewItems.Add(new PlayerListViewItem($"hoge{i}"));
            }
            PlayerListView.ItemsSource = playerListViewItems;
        }

        private void PlayerListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            ListView listView = (ListView)sender;
            PlayerListViewItem playerListViewItem = (PlayerListViewItem)listView.SelectedItem;
            listView.SelectedItem = null;
            //Navigation.PushModalAsync(new NavigationPage(new )
        }

        private async void RegisterPlayerButton_Clicked(object sender, EventArgs e) {
            string playerName = await DisplayPromptAsync("雀士名", "登録する雀士の名前を入力してください", "OK", "Cancel", "hoge", 64, Keyboard.Text, "");
        }
    }
    public class PlayerListViewItem {
        public PlayerListViewItem(string player) {
            Player = player;
        }
        public string Player { get; }
    }
}