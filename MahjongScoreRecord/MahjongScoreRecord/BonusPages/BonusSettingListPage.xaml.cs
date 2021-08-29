﻿using MahjongScoreRecord.Models;
using SQLite;
using System;
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
                Globals.SetCurrentFourPlayersBonusID(selectedBonus.BonusID);
                using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                    SetCurrentBonusColor(db);
                }
                await DisplayAlert("ウマ・オカ変更", $"{selectedBonus.PrizeSettingText}\n1家:{selectedBonus.Bonus1} 2家:{selectedBonus.Bonus2} 3家:{selectedBonus.Bonus3} 4家:{selectedBonus.Bonus4}に変更しました", "OK");
            }
            ((ListView)sender).SelectedItem = null;
        }
        private void SetCurrentBonusColor(SQLiteConnection db) {
            List<BonusListViewItem> bonusListViewItems = new List<BonusListViewItem>();
            int bonusID = Globals.GetCurrentFourPlayersBonusID();
            db.Table<FourPlayersBonus>().ToList().ForEach(bonus => {
                if (bonus.BonusID == bonusID) {
                    bonusListViewItems.Add(new BonusListViewItem(bonus, Color.LightYellow));
                } else {
                    bonusListViewItems.Add(new BonusListViewItem(bonus, Color.White));
                }
            });
            BonusListView.ItemsSource = bonusListViewItems;
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
        }

        private void EditButton_Clicked(object sender, EventArgs e) {

        }
    }
}