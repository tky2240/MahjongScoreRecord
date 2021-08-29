using MahjongScoreRecord.Models;
using PCLStorage;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace MahjongScoreRecord {
    public enum BonusSettingActions {
        None = 0,
        Cancel = 1,
        Edit = 2,
        Set = 3,
    }
    public enum StoreIDs {
        FourPlayerBonus = 1,
        ThreePlayerBonus = 2,
    }
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
        public static int GetCurrentFourPlayersBonusID() {
            return (int)Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()];
        }
        public static void SetCurrentFourPlayersBonusID(int bonusID) {
            Application.Current.Properties[StoreIDs.FourPlayerBonus.ToString()] = bonusID;
        }
        public static int GetCurrentThreePlayersBonusID() {
            return (int)Application.Current.Properties[StoreIDs.ThreePlayerBonus.ToString()];
        }
        public static void SetCurrentThreePlayersBonusID(int bonusID) {
            Application.Current.Properties[StoreIDs.ThreePlayerBonus.ToString()] = bonusID;
        }
    }
    public static class PointStringConverter {
        public static string DeleteNonNumericCharacterWithMinus(string input) {
            string output;
            if (!int.TryParse(input, out int bonus)) {
                if (Regex.IsMatch(input, @"^-.*$")) {
                    output = "-" + Regex.Replace(input.Substring(1), @"[^\d]*", "");
                } else {
                    output = Regex.Replace(input, @"[^\d]*", "");
                }
            } else {
                output = bonus.ToString();
            }
            return output;
        }
        public static string DeleteNonNumericCharacter(string input) {
            string output;
            if (!int.TryParse(input, out int bonus)) {
                output = Regex.Replace(input, @"[^\d]*", "");
            } else {
                output = bonus.ToString();
            }
            return output;
        }
        public static string MinusSymbolToZero(string input) {
            if (input == "-") {
                return "0";
            } else {
                return input;
            }
        }
    }
    public class DBOperations {
        public static async Task CreateDB() {
            IFile dbFile;
            if (await Globals.rootFolder.CheckExistsAsync("Records.sqlite3") == ExistenceCheckResult.FileExists) {
                dbFile = await Globals.rootFolder.CreateFileAsync(Globals.dbFileName, CreationCollisionOption.OpenIfExists);
            } else {
                dbFile = await Globals.rootFolder.CreateFileAsync(Globals.dbFileName, CreationCollisionOption.ReplaceExisting);
            }
            using (SQLiteConnection db = new SQLiteConnection(dbFile.Path)) {
                db.CreateTables(types: new Type[] { typeof(Player), typeof(FourPlayersRecord), typeof(FourPlayersRecordDetail), typeof(ThreePlayersRecord), typeof(ThreePlayersRecordDetail), typeof(FourPlayersBonus), typeof(ThreePlayersBonus) });
            }
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
        private readonly int _Bonus1;
        private readonly int _Bonus2;
        private readonly int _Bonus3;
        private readonly int _Bonus4;
        private readonly int _OriginPoint;
        private readonly int _ReferencePoint;
        public AdjustmentPoints(PlayerPoints playerPoints, PlayerWinds playerWinds, FourPlayersBonus fourPlayersBonus) {
            _OriginPoint = fourPlayersBonus.OriginPoint;
            _ReferencePoint = fourPlayersBonus.ReferencePoint;
            _Bonus1 = fourPlayersBonus.Bonus1;
            _Bonus2 = fourPlayersBonus.Bonus2;
            _Bonus3 = fourPlayersBonus.Bonus3;
            _Bonus4 = fourPlayersBonus.Bonus4;
            List<(int player, int wind, int point)> pointAndPlayers = new List<(int, int, int)>() { ( 1, (int)playerWinds.PlayerWind1, playerPoints.PlayerPoint1 ),
                                                                                                    ( 2, (int)playerWinds.PlayerWind2, playerPoints.PlayerPoint2 ),
                                                                                                    ( 3, (int)playerWinds.PlayerWind3, playerPoints.PlayerPoint3 ),
                                                                                                    ( 4, (int)playerWinds.PlayerWind4, playerPoints.PlayerPoint4 )};
            List<(int player, int wind, int point)> sortedPointAndPlayers = pointAndPlayers.OrderBy(pointAndPlayer => pointAndPlayer.wind).OrderByDescending(pointAndPlayer => pointAndPlayer.point).ToList();
            sortedPointAndPlayers[0] = (sortedPointAndPlayers[0].player, sortedPointAndPlayers[0].wind, sortedPointAndPlayers[0].point + _Bonus1 * 1000 + (_ReferencePoint - _OriginPoint) * 4);
            sortedPointAndPlayers[1] = (sortedPointAndPlayers[1].player, sortedPointAndPlayers[1].wind, sortedPointAndPlayers[1].point + _Bonus2 * 1000);
            sortedPointAndPlayers[2] = (sortedPointAndPlayers[2].player, sortedPointAndPlayers[2].wind, sortedPointAndPlayers[2].point + _Bonus3 * 1000);
            sortedPointAndPlayers[3] = (sortedPointAndPlayers[3].player, sortedPointAndPlayers[3].wind, sortedPointAndPlayers[3].point + _Bonus4 * 1000);

            AdjustmentPoint1 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 1).point;
            AdjustmentPoint2 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 2).point;
            AdjustmentPoint3 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 3).point;
            AdjustmentPoint4 = sortedPointAndPlayers.First(pointAndPlayer => pointAndPlayer.player == 4).point;
            AdjustmentScore1 = Math.Round((AdjustmentPoint1 - _ReferencePoint) / 1000.0, 1, MidpointRounding.AwayFromZero);
            AdjustmentScore2 = Math.Round((AdjustmentPoint2 - _ReferencePoint) / 1000.0, 1, MidpointRounding.AwayFromZero);
            AdjustmentScore3 = Math.Round((AdjustmentPoint3 - _ReferencePoint) / 1000.0, 1, MidpointRounding.AwayFromZero);
            AdjustmentScore4 = Math.Round((AdjustmentPoint4 - _ReferencePoint) / 1000.0, 1, MidpointRounding.AwayFromZero);
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
