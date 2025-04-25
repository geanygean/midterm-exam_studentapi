using static System.Collections.Specialized.BitVector32;

namespace StudentAPI.Models
{
	public class Subject
	{
		public int Id { get; set; }
		public string Code { get; set; } = "";
		public string Description { get; set; } = "";
		public ICollection<Section> Sections { get; set; }
	}

}
