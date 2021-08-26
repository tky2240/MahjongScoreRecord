using MahjongScoreRecord.Models;
using PCLStorage;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MahjongScoreRecord {
    public enum Winds {
        None = 0,
        East = 1,
        South = 2,
        West = 3,
        North = 4,
    }
    public class Globals {
        public static string dbFileName = "Records.sqlite3";
        public static IFolder rootFolder = FileSystem.Current.LocalStorage;
        public static List<KeyValuePair<Winds, string>> winds = new List<KeyValuePair<Winds, string>>()
        {
            new KeyValuePair<Winds , string>(Winds.East,"東" ),
            new KeyValuePair<Winds , string>(Winds.South,"南" ),
            new KeyValuePair<Winds , string>(Winds.West,"西" ),
            new KeyValuePair<Winds , string>(Winds.North,"北" )
        };
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
            db.CreateTables(types: new Type[] { typeof(Player), typeof(FourPlayersRecord), typeof(FourPlayersRecordDetail), typeof(ThreePlayersRecord), typeof(ThreePlayersRecordDetail), typeof(FourPlayersBonus), typeof(ThreePlayersBonus) });
            db.Dispose();
            return;
        }
        public static async Task<SQLiteConnection> ConnectDB() {
            IFile dbFile = await Globals.rootFolder.CreateFileAsync(Globals.dbFileName, CreationCollisionOption.OpenIfExists);
            SQLiteConnection db = new SQLiteConnection(dbFile.Path);
            return db;
        }
    }
    public class PlayerNames {
        public PlayerNames(string playerName1, string playerName2, string playerName3, string playerName4) {
            PlayerName1 = playerName1;
            PlayerName2 = playerName2;
            PlayerName3 = playerName3;
            PlayerName4 = playerName4;
        }
        public string PlayerName1 { get; }
        public string PlayerName2 { get; }
        public string PlayerName3 { get; }
        public string PlayerName4 { get; }
    }
    public class PlayerWinds {
        public PlayerWinds(Winds playerWind1, Winds playerWind2, Winds playerWind3, Winds playerWind4) {
            PlayerWind1 = playerWind1;
            PlayerWind2 = playerWind2;
            PlayerWind3 = playerWind3;
            PlayerWind4 = playerWind4;
        }
        public Winds PlayerWind1 { get; }
        public Winds PlayerWind2 { get; }
        public Winds PlayerWind3 { get; }
        public Winds PlayerWind4 { get; }

    }
    public class PlayerPoints {
        public PlayerPoints(int playerPoint1, int playerPoint2, int playerPoint3, int playerPoint4) {
            PlayerPoint1 = playerPoint1;
            PlayerPoint2 = playerPoint2;
            PlayerPoint3 = playerPoint3;
            PlayerPoint4 = playerPoint4;
        }
        public int PlayerPoint1 { get; }
        public int PlayerPoint2 { get; }
        public int PlayerPoint3 { get; }
        public int PlayerPoint4 { get; }
    }
    public class AdjustmentPoints {
        private readonly int Bonus1 = 20000;
        private readonly int Bonus2 = 10000;
        private readonly int Bonus3 = -10000;
        private readonly int Bonus4 = -20000;
        private readonly int TopPrize = 20000;
        private readonly int Kaeshi = 30000;
        public AdjustmentPoints(PlayerPoints playerPoints, PlayerWinds playerWinds) {
            List<(int player, int wind, int point)> pointAndPlayers = new List<(int, int, int)>() { ( 1, (int)playerWinds.PlayerWind1, playerPoints.PlayerPoint1 ),
                                                                                                    ( 2, (int)playerWinds.PlayerWind2, playerPoints.PlayerPoint2 ),
                                                                                                    ( 3, (int)playerWinds.PlayerWind3, playerPoints.PlayerPoint3 ),
                                                                                                    ( 4, (int)playerWinds.PlayerWind4, playerPoints.PlayerPoint4 )};
            List<(int player, int wind, int point)> sortedPointAndPlayers = pointAndPlayers.OrderBy(pointAndPlayer => pointAndPlayer.wind).OrderByDescending(pointAndPlayer => pointAndPlayer.point).ToList();
            sortedPointAndPlayers[0] = (sortedPointAndPlayers[0].player, sortedPointAndPlayers[0].wind, sortedPointAndPlayers[0].point + Bonus1 + TopPrize);
            sortedPointAndPlayers[1] = (sortedPointAndPlayers[1].player, sortedPointAndPlayers[1].wind, sortedPointAndPlayers[1].point + Bonus2);
            sortedPointAndPlayers[2] = (sortedPointAndPlayers[2].player, sortedPointAndPlayers[2].wind, sortedPointAndPlayers[2].point + Bonus3);
            sortedPointAndPlayers[3] = (sortedPointAndPlayers[3].player, sortedPointAndPlayers[3].wind, sortedPointAndPlayers[3].point + Bonus4);

            AdjustmentPoint1 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 1).point;
            AdjustmentPoint2 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 2).point;
            AdjustmentPoint3 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 3).point;
            AdjustmentPoint4 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 4).point;
            AdjustmentScore1 = Math.Truncate((AdjustmentPoint1 - Kaeshi) / 100.0) / 10;
            AdjustmentScore2 = Math.Truncate((AdjustmentPoint2 - Kaeshi) / 100.0) / 10;
            AdjustmentScore3 = Math.Truncate((AdjustmentPoint3 - Kaeshi) / 100.0) / 10;
            AdjustmentScore4 = Math.Truncate((AdjustmentPoint4 - Kaeshi) / 100.0) / 10;
        }
        public int AdjustmentPoint1 { get; }
        public int AdjustmentPoint2 { get; }
        public int AdjustmentPoint3 { get; }
        public int AdjustmentPoint4 { get; }
        public double AdjustmentScore1 { get; }
        public double AdjustmentScore2 { get; }
        public double AdjustmentScore3 { get; }
        public double AdjustmentScore4 { get; }
    }
}
