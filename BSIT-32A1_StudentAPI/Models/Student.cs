namespace StudentAPI.Models
{
	public class Student
	{
		public int Id { get; set; }
		public string FirstName { get; set; } = "";
		public string LastName { get; set; } = "";
		public string Email { get; set; } = "";
		public ICollection<StudentSection> StudentSections { get; set; }
	}

}
