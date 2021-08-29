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
            if (!string.IsNullOrEmpty(bonusEntry.Text)) {
                if (!int.TryParse(bonusEntry.Text, out int bonus)) {
                    if (Regex.IsMatch(bonusEntry.Text, @"^-.*$")) {
                        bonusEntry.Text = "-" + Regex.Replace(bonusEntry.Text.Substring(1), @"[^\d]*", "");
                    } else {
                        bonusEntry.Text = Regex.Replace(bonusEntry.Text, @"[^\d]*", "");
                    }
                } else {
                    bonusEntry.Text = bonus.ToString();
                }
            }
        }
        private void BonusEntry_Unfocused(object sender, FocusEventArgs e) {
            Entry bonusEntry = (Entry)sender;
            if (bonusEntry.Text == "-") {
                bonusEntry.Text = "0";
            }
        }
        private void PointEntry_TextChanged(object sender, TextChangedEventArgs e) {
            Entry bonusEntry = (Entry)sender;
            if (!string.IsNullOrEmpty(bonusEntry.Text)) {
                if (!int.TryParse(bonusEntry.Text, out int bonus)) {
                    bonusEntry.Text = Regex.Replace(bonusEntry.Text, @"[^\d]*", "");
                } else {
                    bonusEntry.Text = bonus.ToString();
                }
            }
        }
        private async void RegisterButton_Clicked(object sender, EventArgs e) {
            if (_BonusEntries.Any(bonus => string.IsNullOrEmpty(bonus.Text)) || _PointEntries.Any(point => string.IsNullOrEmpty(point.Text))) {
                await DisplayAlert("エラー", "ウマ・オカ、原点・返しを正しく入力してください", "OK");
            } else {
                using (SQLiteConnection db = await DBOperations.ConnectDB()) {
                    db.Insert(new FourPlayersBonus() {
                        OriginPoint = int.Parse(OriginPointEntry.Text),
                        ReferencePoint = int.Parse(ReferencePointEntry.Text),
                        Bonus1 = int.Parse(BonusEntry1.Text),
                        Bonus2 = int.Parse(BonusEntry2.Text),
                        Bonus3 = int.Parse(BonusEntry3.Text),
                        Bonus4 = int.Parse(BonusEntry4.Text)
                    });
                    Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()] = db.Table<FourPlayersBonus>().Last().BonusID;
                }
                await Navigation.PopModalAsync(true);
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e) {
            await Navigation.PopModalAsync(true);
        }
    }
}