using System.Threading.Tasks;
using Xamarin.Forms;

namespace MahjongScoreRecord {
    public partial class App : Application {
        public App() {
            InitializeComponent();
            Task loadTask = DBOperations.CreateDB();
            MainPage = new NavigationPage(new LoadingPage(loadTask, new MainPage()));
        }
        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
