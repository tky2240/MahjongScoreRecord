using MahjongScoreRecord.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MahjongScoreRecord {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BonusSettingUpdatePage : ContentPage {
        private readonly List<Entry> _BonusEntries;
        private readonly List<Entry> _PointEntries;
        private readonly int _BonusID;
        public BonusSettingUpdatePage(int bonusID) {
            InitializeComponent();
            _BonusEntries = new List<Entry>() { BonusEntry1, BonusEntry2, BonusEntry3, BonusEntry4 };
            _PointEntries = new List<Entry>() { OriginPointEntry, ReferencePointEntry };
            _BonusID = bonusID;
        }
        private async void BonusSettingUpdatePage_Appearing(object sender, EventArgs e) {
            using(SQLiteConnection db = await DBOperations.ConnectDB()) {
                FourPlayersBonus fourPlayersBonus = db.Table<FourPlayersBonus>().First(bonus => bonus.BonusID == _BonusID);
                OriginPointEntry.Text = fourPlayersBonus.OriginPoint.ToString(); ;
                ReferencePointEntry.Text = fourPlayersBonus.ReferencePoint.ToString();
                BonusEntry1.Text = fourPlayersBonus.Bonus1.ToString();
                BonusEntry2.Text = fourPlayersBonus.Bonus2.ToString();
                BonusEntry3.Text = fourPlayersBonus.Bonus3.ToString();
                BonusEntry4.Text = fourPlayersBonus.Bonus4.ToString();
            }
        }
        private void BonusEntry_TextChanged(object sender, TextChangedEventArgs e) {
            Entry bonusEntry = (Entry)sender;
            if (!string.IsNullOrWhiteSpace(bonusEntry.Text)) {
                bonusEntry.Text = PointStringConverter.DeleteNonNumericCharacterWithMinus(bonusEntry.Text);
            }
        }
        private void BonusEntry_Unfocused(object sender, FocusEventArgs e) {
            Entry bonusEntry = (Entry)sender;
            bonusEntry.Text = PointStringConverter.MinusSymbolToZero(bonusEntry.Text);
        }
        private void PointEntry_TextChanged(object sender, TextChangedEventArgs e) {
            Entry pointEntry = (Entry)sender;
            if (!string.IsNullOrWhiteSpace(pointEntry.Text)) {
                pointEntry.Text = PointStringConverter.DeleteNonNumericCharacter(pointEntry.Text);
            }
        }
        private async void UpdateButton_Clicked(object sender, EventArgs e) {
            if (_BonusEntries.Any(bonus => string.IsNullOrEmpty(bonus.Text)) || _PointEntries.Any(point => string.IsNullOrEmpty(point.Text))) {
                await DisplayAlert("エラー", "ウマ・オカ、原点・返しを正しく入力してください", "OK");
                return;
            }
            if (int.Parse(BonusEntry1.Text) + int.Parse(BonusEntry2.Text) + int.Parse(BonusEntry3.Text) + int.Parse(BonusEntry4.Text) != 0) {
                if (!await DisplayAlert("確認", "ウマの合計が0になりませんがこのまま登録しますか？", "Yes", "No")) {
                    return;
                }
            }
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                db.Update(new FourPlayersBonus() {
                    BonusID = _BonusID,
                    OriginPoint = int.Parse(OriginPointEntry.Text),
                    ReferencePoint = int.Parse(ReferencePointEntry.Text),
                    Bonus1 = int.Parse(BonusEntry1.Text),
                    Bonus2 = int.Parse(BonusEntry2.Text),
                    Bonus3 = int.Parse(BonusEntry3.Text),
                    Bonus4 = int.Parse(BonusEntry4.Text)
                });
            }
            await Navigation.PopModalAsync(true);
        }
        private async void DeleteButton_Clicked(object sender, EventArgs e) {
            using(SQLiteConnection db = await DBOperations.ConnectDB()) {
                if (db.Table<FourPlayersBonus>().Count() < 2) {
                    await DisplayAlert("エラー", "削除後の設定が0個になってしまうため削除できません", "OK");
                } else {
                    if (await DisplayAlert("削除確認", "表示している設定を削除してもよろしいですか？", "Yes", "No")) {
                        db.Table<FourPlayersBonus>().Delete(bonus => bonus.BonusID == _BonusID);
                        Globals.SetCurrentFourPlayersBonusID(db.Table<FourPlayersBonus>().First().BonusID);
                        await Navigation.PopModalAsync(true);
                    }
                }
            }
        }
        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
        
    }
}