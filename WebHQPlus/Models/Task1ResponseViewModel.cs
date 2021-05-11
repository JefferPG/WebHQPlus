using System.Collections.Generic;

namespace WebHQTest.Models
{
	public class Task1ResponseViewModel : GeneralResponseViewModel
	{
		public Task1ResponseViewModel()
		{
			RoomCategory = new List<string>();
			Alternative = new List<AlternativeHotel>();
		}

		public string Url { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public int Clasification { get; set; }
		public decimal Review { get; set; }
		public string Description { get; set; }
		public List<string> RoomCategory { get; set; }
		public List<AlternativeHotel> Alternative { get; set; }

		public class AlternativeHotel
		{
			public string Url { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public decimal Review { get; set; }

		}
	}
}
