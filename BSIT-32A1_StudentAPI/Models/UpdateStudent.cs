namespace StudentAPI.Models
{
	public class UpdateStudent
	{
		public int Id { get; set; }      
		public string FirstName { get; set; } 
		public string LastName { get; set; }  
		public string Email { get; set; }  
		public int SectionId { get; set; } 
	}
}
