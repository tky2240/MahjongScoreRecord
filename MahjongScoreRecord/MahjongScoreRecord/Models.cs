﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MahjongScoreRecord.Models {
	[Table("Playaer")]
	public class Player {
		[PrimaryKey, AutoIncrement]
		public int PlayerID { get; set; }
		public string PlayerName { get; set; }
	}
	[Table("FourPlayersRecord")]
	public class FourPlayersRecord {
		[PrimaryKey, AutoIncrement]
		public int RecordID { get; set; }
		public string RecordName { get; set; }
		public DateTime RecordTime { get; set; }
		public int PlayerID1 { get; set; }
		public int PlayerID2 { get; set; }
		public int PlayerID3 { get; set; }
		public int PlayerID4 { get; set; }
	}
	[Table("FourPlayersRecordDetail")]
	public class FourPlayersRecordDetail {
		[PrimaryKey, AutoIncrement]
		public int RecordDetailID { get; set; }
		public int RecordID { get; set; }
		public int PlayerPoint1 { get; set; }
		public int PlayerPoint2 { get; set; }
		public int PlayerPoint3 { get; set; }
		public int PlayerPoint4 { get; set; }
		public int MatchCount { get; set; }
	}
	[Table("ThreePlayersRecord")]
	public class ThreePlayersRecord {
		[PrimaryKey, AutoIncrement]
		public int RecordID { get; set; }
		public string RecordName { get; set; }
		public DateTime RecordTime { get; set; }
		public int PlayerID1 { get; set; }
		public int PlayerID2 { get; set; }
		public int PlayerID3 { get; set; }
	}
	[Table("ThreePlayersRecordDetail")]
	public class ThreePlayersRecordDetail {
		[PrimaryKey, AutoIncrement]
		public int RecordDetailID { get; set; }
		public int RecordID { get; set; }
		public int PlayerPoint1 { get; set; }
		public int PlayerPoint2 { get; set; }
		public int PlayerPoint3 { get; set; }
		public int MatchCount { get; set; }
	}
}
