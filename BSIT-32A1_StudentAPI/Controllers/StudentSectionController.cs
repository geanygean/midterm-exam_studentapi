using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StudentSectionsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public StudentSectionsController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<object>>> GetStudentSections()
		{
			var result = await _context.StudentSections
				.Include(ss => ss.Student)
				.Include(ss => ss.Section)
					.ThenInclude(sec => sec.Subject)
				.Select(ss => new
				{
					ss.StudentId,
					student = new
					{
						id = ss.Student!.Id,
						ss.Student.FirstName,
						ss.Student.LastName,
						ss.Student.Email
					},
					ss.SectionId,
					section = new
					{
						id = ss.Section!.Id,
						ss.Section.Name,
						ss.Section.SubjectId,
						subject = new
						{
							id = ss.Section.Subject!.Id,
							ss.Section.Subject.Code,
							ss.Section.Subject.Description
						}
					}
				})
				.ToListAsync();

			return Ok(result);
		}

		[HttpPost]
		public async Task<ActionResult> CreateStudentSection(StudentSection dto)
		{
			_context.StudentSections.Add(dto);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetStudentSections), null);
		}

		[HttpDelete("{studentId}/{sectionId}")]
		public async Task<IActionResult> DeleteStudentSection(int studentId, int sectionId)
		{
			var ss = await _context.StudentSections.FindAsync(studentId, sectionId);
			if (ss == null)
				return NotFound();

			_context.StudentSections.Remove(ss); // Permanently delete
			await _context.SaveChangesAsync();

			return NoContent();
		}

	}
}
