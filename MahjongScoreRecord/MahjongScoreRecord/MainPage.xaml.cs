using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MahjongScoreRecord {
    public partial class MainPage : TabbedPage {
        public MainPage() {
            InitializeComponent();
            this.Children.Add(new NavigationPage(new RecordListPage()));
        }

        private void Button_Clicked(object sender, EventArgs e) {
            Navigation.PushModalAsync(new NavigationPage(new RecordListPage()), true);
        }
    }
}
