using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage {
        public LoadingPage(Task loadTask, Page nextPage) {
            InitializeComponent();
            waitLoading(loadTask, nextPage);
        }

        private async void waitLoading(Task loadTask, Page nextPage) {
            await loadTask;
            await Task.Delay(1000);
            Navigation.PushModalAsync(new NavigationPage(nextPage), true);
        }
    }
}