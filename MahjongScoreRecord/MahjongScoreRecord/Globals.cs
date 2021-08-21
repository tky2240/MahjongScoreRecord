using System;
using System.Collections.Generic;
using System.Text;
using PCLStorage;
using SQLite;
using System.Threading.Tasks;
using MahjongScoreRecord.Models;

namespace MahjongScoreRecord {
    public class Globals {
        public static string dbFileName = "Records.sqlite3";
        public static IFolder rootFolder = FileSystem.Current.LocalStorage;
    }
    public class DBOperations {
        public static async Task CreateDB() {
            IFile dbFile;
            if (await Globals.rootFolder.CheckExistsAsync("Records.sqlite3") == ExistenceCheckResult.FileExists) {
                dbFile = await Globals.rootFolder.CreateFileAsync(Globals.dbFileName, CreationCollisionOption.OpenIfExists);
            } else {
                dbFile = await Globals.rootFolder.CreateFileAsync(Globals.dbFileName, CreationCollisionOption.ReplaceExisting);
            }
            SQLiteConnection db = new SQLiteConnection(dbFile.Path);
            db.CreateTables(types: new Type[] { typeof(Player), typeof(FourPlayersRecord), typeof(FourPlayersRecordDetail), typeof(ThreePlayersRecord), typeof(ThreePlayersRecordDetail) });
            db.Dispose();
            return;
        }
        public static async Task<SQLiteConnection> ConnectDB() {
            IFile dbFile = await Globals.rootFolder.CreateFileAsync(Globals.dbFileName, CreationCollisionOption.OpenIfExists);
            SQLiteConnection db = new SQLiteConnection(dbFile.Path);
            return db;
        }
    }
}
