using System.Collections.Generic;

namespace WebHQTest.Models
{
	public class HotelRatesViewModel
	{
		public HotelDescription Hotel { get; set; }
		public IEnumerable<HotelRate> HotelRates { get; set; }

		public class HotelDescription
		{
			public int HotelID { get; set; }
			public int Classification { get; set; }
			public string Name { get; set; }
			public decimal Reviewscore { get; set; }
		}

		public class HotelRate
		{
			public int Adults { get; set; }
			public int Los { get; set; }
			public PriceInfo Price { get; set; }
			public string RateDescription { get; set; }
			public string RrateID { get; set; }
			public string RateName { get; set; }
			public IEnumerable<RateTag> RateTags { get; set; }
			public string TargetDay { get; set; }

			public class PriceInfo
			{
				public string Currency { get; set; }
				public decimal NumericFloat { get; set; }
				public int NumericInteger { get; set; }

			}

			public class RateTag
			{
				public string Name { get; set; }
				public bool Shape { get; set; }

			}
		}
	}
}