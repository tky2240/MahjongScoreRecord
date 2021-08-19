using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MahjongScoreRecord.Models {
	public class Player {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class FourPlayersRecord {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime RecordTime { get; set; }
		public string Player1Id { get; set; }
		public string Player2Id { get; set; }
		public string Player3Id { get; set; }
		public string Player4Id { get; set; }
	}
	public class FourPlayersRecordDetail {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int RecordId { get; set; }
		public int Player1Point { get; set; }
		public int Player2Point { get; set; }
		public int Player3Point { get; set; }
		public int Player4Point { get; set; }
	}
	public class ThreePlayersRecord {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime RecordTime { get; set; }
		public string Player1Id { get; set; }
		public string Player2Id { get; set; }
		public string Player3Id { get; set; }
	}
	public class ThreePlayersRecordDetail {
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public int RecordId { get; set; }
		public int Player1Point { get; set; }
		public int Player2Point { get; set; }
		public int Player3Point { get; set; }
		public int Player4Point { get; set; }
	}
}
