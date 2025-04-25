using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StudentsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public StudentsController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<object>>> GetStudents()
		{
			var students = await _context.Students
				.Include(s => s.StudentSections)
					.ThenInclude(ss => ss.Section)
					.ThenInclude(sec => sec.Subject)
				.Select(s => new
				{
					StudentId = s.Id,
					s.FirstName,
					s.LastName,
					s.Email,
					Section = s.StudentSections.Select(ss => new
					{
						ss.Section.Name,
						Subject = ss.Section.Subject != null ? new
						{
							ss.Section.Subject.Description
						} : null
					}).FirstOrDefault()
				})
				.ToListAsync();

			return Ok(students);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<object>> GetStudent(int id)
		{
			var student = await _context.Students
				.Include(s => s.StudentSections)
					.ThenInclude(ss => ss.Section)
					.ThenInclude(sec => sec.Subject)
				.Where(s => s.Id == id)
				.Select(s => new
				{
					StudentId = s.Id,
					s.FirstName,
					s.LastName,
					s.Email,
					Section = s.StudentSections.Select(ss => new
					{
						ss.Section.Name,
						Subject = ss.Section.Subject != null ? new
						{
							ss.Section.Subject.Description
						} : null
					}).FirstOrDefault()
				})
				.FirstOrDefaultAsync();

			if (student == null)
				return NotFound();

			return Ok(student);
		}

		[HttpPost]
		public async Task<ActionResult> CreateStudent(CreateStudent dto)
		{
			if (await _context.Students.AnyAsync(s => s.Email == dto.Email))
				return BadRequest("Student with this email already exists.");

			if (await _context.Students.AnyAsync(s => s.FirstName == dto.FirstName && s.LastName == dto.LastName))
				return BadRequest("A student with the same first name and last name already exists.");

			var student = new Student
			{
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				Email = dto.Email
			};
			_context.Students.Add(student);
			await _context.SaveChangesAsync();

			var ss = new StudentSection
			{
				StudentId = student.Id,
				SectionId = dto.SectionId
			};
			_context.StudentSections.Add(ss);
			await _context.SaveChangesAsync();

			var studentWithSection = await _context.Students
				.Where(s => s.Id == student.Id)
				.Include(s => s.StudentSections)
					.ThenInclude(ss => ss.Section)
						.ThenInclude(sec => sec.Subject)
				.Select(s => new
				{
					s.Id,
					s.FirstName,
					s.LastName,
					s.Email,
					Section = s.StudentSections
						.Select(ss => new
						{
							ss.Section.Id,
							ss.Section.Name,
							Subject = new
							{
								ss.Section.Subject.Id,
								ss.Section.Subject.Code,
								ss.Section.Subject.Description
							}
						})
						.FirstOrDefault()
				})
				.FirstOrDefaultAsync();

			return CreatedAtAction(
				nameof(GetStudent),
				new { id = student.Id },
				studentWithSection
			);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateStudent(int id, UpdateStudent studentDto)
		{
			if (id != studentDto.Id) return BadRequest("Student ID mismatch.");

			var student = await _context.Students.FindAsync(id);
			if (student == null) return NotFound();

			student.FirstName = studentDto.FirstName;
			student.LastName = studentDto.LastName;
			student.Email = studentDto.Email;

			var existingSection = await _context.StudentSections
				.FirstOrDefaultAsync(ss => ss.StudentId == id);

			if (existingSection != null)
			{
				_context.StudentSections.Remove(existingSection);
			}

			var ss = new StudentSection
			{
				StudentId = student.Id,
				SectionId = studentDto.SectionId
			};
			_context.StudentSections.Add(ss);

			_context.Entry(student).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> HardDeleteStudent(int id)
		{
			var student = await _context.Students
				.Include(s => s.StudentSections)
				.FirstOrDefaultAsync(s => s.Id == id);

			if (student == null)
			{
				return NotFound();
			}

			var studentSections = await _context.StudentSections
				.Where(ss => ss.StudentId == id)
				.ToListAsync();
			_context.StudentSections.RemoveRange(studentSections);

			_context.Students.Remove(student);
			await _context.SaveChangesAsync();

			return NoContent();
		}

	}
}
