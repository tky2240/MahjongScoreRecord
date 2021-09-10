using MahjongScoreRecord.Models;
using SQLite;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BonusSettingListPage : ContentPage {
        public BonusSettingListPage() {
            InitializeComponent();
        }

        private async void BonusSettingPage_Appearing(object sender, EventArgs e) {
            if (Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                FourPlayersModeRadioButton.IsChecked = true;
            } else if (Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                ThreePlayersModeRadioButton.IsChecked = true;
            }
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                SetCurrentBonusColor(db);
            }
        }

        private async void BonusListView_ItemTapped(object sender, ItemTappedEventArgs e) {
            BonusListViewItem selectedBonus= (BonusListViewItem)((ListView)sender).SelectedItem;
            if(selectedBonus == null) {
                return;
            }
            string selectedActionString =  await DisplayActionSheet("操作を選んでください", BonusSettingActions.Cancel.ToString(), null, BonusSettingActions.Edit.ToString(), BonusSettingActions.Set.ToString());
            if(selectedActionString == BonusSettingActions.Edit.ToString()) {
                await Navigation.PushModalAsync(new NavigationPage(new BonusSettingUpdatePage(selectedBonus.BonusID)));
            }else if(selectedActionString == BonusSettingActions.Set.ToString()){
                if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                    await Globals.SetCurrentFourPlayersBonusIDAsync(selectedBonus.BonusID);
                    await DisplayAlert("ウマ・オカ変更", $"{selectedBonus.PrizeSettingText}\n1家:{selectedBonus.Bonus1} 2家:{selectedBonus.Bonus2} 3家:{selectedBonus.Bonus3} 4家:{selectedBonus.Bonus4}に変更しました", "OK");
                }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                    await Globals.SetCurrentThreePlayersBonusIDAsync(selectedBonus.BonusID);
                    await DisplayAlert("ウマ・オカ変更", $"{selectedBonus.PrizeSettingText}\n1家:{selectedBonus.Bonus1} 2家:{selectedBonus.Bonus2} 3家:{selectedBonus.Bonus3} に変更しました", "OK");
                }
                using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                    SetCurrentBonusColor(db);
                }
            }
            ((ListView)sender).SelectedItem = null;
        }
        private void SetCurrentBonusColor(SQLiteConnection db) {
            List<BonusListViewItem> bonusListViewItems = new List<BonusListViewItem>();
            int bonusID = -1;
            if(Globals.GetCurrentPlayersMode() == PlayersMode.Four) {
                bonusID = Globals.GetCurrentFourPlayersBonusID();
                db.Table<FourPlayersBonus>().ToList().ForEach(bonus => {
                    if (bonus.BonusID == bonusID) {
                        bonusListViewItems.Add(new BonusListViewItem(bonus, Color.LightYellow));
                    } else {
                        bonusListViewItems.Add(new BonusListViewItem(bonus, Color.White));
                    }
                });
            }else if(Globals.GetCurrentPlayersMode() == PlayersMode.Three) {
                bonusID = Globals.GetCurrentThreePlayersBonusID();
                db.Table<ThreePlayersBonus>().ToList().ForEach(bonus => {
                    if (bonus.BonusID == bonusID) {
                        bonusListViewItems.Add(new BonusListViewItem(bonus, Color.LightYellow));
                    } else {
                        bonusListViewItems.Add(new BonusListViewItem(bonus, Color.White));
                    }
                });
            }
            BonusListView.ItemsSource = bonusListViewItems;
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
            BonusSettingPage_Appearing(null, null);
        }
        private async void BonusRegisterButton_Clicked(object sender, EventArgs e) {
            await Navigation.PushModalAsync(new NavigationPage(new BonusSettingRegisterPage()), true);
        }
        private class BonusListViewItem {
            public BonusListViewItem(FourPlayersBonus fourPlayersBonus, Color backGroudColor) {
                BonusID = fourPlayersBonus.BonusID;
                OriginPoint = fourPlayersBonus.OriginPoint;
                ReferencePoint = fourPlayersBonus.ReferencePoint;
                Bonus1 = fourPlayersBonus.Bonus1;
                Bonus2 = fourPlayersBonus.Bonus2;
                Bonus3 = fourPlayersBonus.Bonus3;
                Bonus4 = fourPlayersBonus.Bonus4;
                BonusText1 = $"1家:{Bonus1}";
                BonusText2 = $"2家:{Bonus2}";
                BonusText3 = $"3家:{Bonus3}";
                BonusText4 = $"4家:{Bonus4}";
                PrizeSettingText = $"原点{OriginPoint}の{ReferencePoint}返し";
                BackGroundColor = backGroudColor;
                PlayersMode = PlayersMode.Four;
            }
            public BonusListViewItem(ThreePlayersBonus threePlayersBonus, Color backGroudColor) {
                BonusID = threePlayersBonus.BonusID;
                OriginPoint = threePlayersBonus.OriginPoint;
                ReferencePoint = threePlayersBonus.ReferencePoint;
                Bonus1 = threePlayersBonus.Bonus1;
                Bonus2 = threePlayersBonus.Bonus2;
                Bonus3 = threePlayersBonus.Bonus3;
                Bonus4 = 0;
                BonusText1 = $"1家:{Bonus1}";
                BonusText2 = $"2家:{Bonus2}";
                BonusText3 = $"3家:{Bonus3}";
                BonusText4 = "";
                PrizeSettingText = $"原点{OriginPoint}の{ReferencePoint}返し";
                BackGroundColor = backGroudColor;
                PlayersMode = PlayersMode.Three;
            }
            public int BonusID { get; }
            public string PrizeSettingText { get; }
            public int OriginPoint { get; }
            public int ReferencePoint { get; }
            public int Bonus1 { get; }
            public int Bonus2 { get; }
            public int Bonus3 { get; }
            public int Bonus4 { get; }
            public string BonusText1 { get; }
            public string BonusText2 { get; }
            public string BonusText3 { get; }
            public string BonusText4 { get; }
            public Color BackGroundColor { get; }
            public PlayersMode PlayersMode { get; }
        }
    }
}