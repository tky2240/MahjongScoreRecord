using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage {
        public LoadingPage(Task loadTask, Page nextPage) {
            InitializeComponent();
            WaitLoading(loadTask, nextPage);
        }

        private async void WaitLoading(Task loadTask, Page nextPage) {
            await loadTask;
            await Task.Delay(1000);
            await Navigation.PushModalAsync(new NavigationPage(nextPage), true);
        }
    }
}