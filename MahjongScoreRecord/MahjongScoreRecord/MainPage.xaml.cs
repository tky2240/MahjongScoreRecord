using Xamarin.Forms;

namespace MahjongScoreRecord {
    public partial class MainPage : TabbedPage {
        public MainPage() {
            InitializeComponent();
            RecordListPage.BindingContext = ImageSource.FromResource("MahjongScoreRecord.Images.Record.png");
            PlayerListPage.BindingContext = ImageSource.FromResource("MahjongScoreRecord.Images.Player.png");
            BonusSettingListPage.BindingContext = ImageSource.FromResource("MahjongScoreRecord.Images.Bonus.png");
        }
    }
}