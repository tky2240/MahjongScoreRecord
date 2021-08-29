using MahjongScoreRecord.Models;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage {
        private readonly Task _LoadTask;
        private readonly Page _NextPage;
        public LoadingPage(Task loadTask, Page nextPage) {
            InitializeComponent();
            _LoadTask = loadTask;
            _NextPage = nextPage;
        }
        private async void LoadingPage_Appearing(object sender, System.EventArgs e) {
            await _LoadTask;
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                if (db.Table<FourPlayersBonus>().Count() == 0) {
                    db.Insert(new FourPlayersBonus() {
                        OriginPoint = 25000,
                        ReferencePoint = 30000,
                        Bonus1 = 10,
                        Bonus2 = 5,
                        Bonus3 = -5,
                        Bonus4 = -10
                    });
                }
                if (db.Table<ThreePlayersBonus>().Count() == 0) {
                    db.Insert(new ThreePlayersBonus() {
                        OriginPoint = 35000,
                        ReferencePoint = 40000,
                        Bonus1 = 10,
                        Bonus2 = 0,
                        Bonus3 = -10,
                    });
                }
                if (!Application.Current.Properties.ContainsKey(StoreIDs.FourPlayerBonus.ToString())) {
                    Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()] = db.Table<FourPlayersBonus>().First().BonusID;
                }
                if (!Application.Current.Properties.ContainsKey(StoreIDs.ThreePlayerBonus.ToString())) {
                    Application.Current.Properties[StoreIDs.ThreePlayerBonus.ToString()] = db.Table<ThreePlayersBonus>().First().BonusID;
                }
            }
            Application.Current.MainPage = new NavigationPage(_NextPage);
        }
    }
}