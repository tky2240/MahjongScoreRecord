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
    public partial class BonusSettingRegisterPage : ContentPage {
        private readonly List<Entry> _BonusEntries;
        private readonly List<Entry> _PointEntries;
        public BonusSettingRegisterPage() {
            InitializeComponent();
            _BonusEntries = new List<Entry>() { BonusEntry1, BonusEntry2, BonusEntry3, BonusEntry4 };
            _PointEntries = new List<Entry>() { OriginPointEntry, ReferencePointEntry };
        }

        private void BonusEntry_TextChanged(object sender, TextChangedEventArgs e) {
            Entry bonusEntry = (Entry)sender;
            string input = bonusEntry.Text;
            if (!string.IsNullOrWhiteSpace(input)) {
                bonusEntry.Text = PointStringConverter.DeleteNonNumericCharacterWithMinus(input);
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
        private async void RegisterButton_Clicked(object sender, EventArgs e) {
            if (_BonusEntries.Any(bonus => string.IsNullOrEmpty(bonus.Text)) || _PointEntries.Any(point => string.IsNullOrEmpty(point.Text))) {
                await DisplayAlert("エラー", "ウマ・オカ、原点・返しを正しく入力してください", "OK");
                return;
            }
            if(int.Parse(BonusEntry1.Text) + int.Parse(BonusEntry2.Text) + int.Parse(BonusEntry3.Text) + int.Parse(BonusEntry4.Text) != 0) {
                if(!await DisplayAlert("確認", "ウマの合計が0になりませんがこのまま登録しますか？", "Yes", "No")) {
                    return;
                }
            }
            using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                db.Insert(new FourPlayersBonus() {
                    OriginPoint = int.Parse(OriginPointEntry.Text),
                    ReferencePoint = int.Parse(ReferencePointEntry.Text),
                    Bonus1 = int.Parse(BonusEntry1.Text),
                    Bonus2 = int.Parse(BonusEntry2.Text),
                    Bonus3 = int.Parse(BonusEntry3.Text),
                    Bonus4 = int.Parse(BonusEntry4.Text)
                });
                Globals.SetCurrentFourPlayersBonusID(db.Table<FourPlayersBonus>().Last().BonusID);
            }
            await Navigation.PopModalAsync(true);
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}