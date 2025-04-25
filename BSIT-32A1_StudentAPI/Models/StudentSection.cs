namespace StudentAPI.Models
{
	public class StudentSection
	{
		public int StudentId { get; set; }
		public Student Student { get; set; }

		public int SectionId { get; set; }
		public Section Section { get; set; }
	}

}
