using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using SQLite;
using MahjongScoreRecord.Models;
using System.Collections.Generic;
using PCLStorage;
using System.Threading.Tasks;

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
