using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubjectsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public SubjectsController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
		{
			var subjects = await _context.Subjects
				.Select(s => new Subject
				{
					Id = s.Id,
					Code = s.Code,
					Description = s.Description
				})
				.ToListAsync();

			return Ok(subjects);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Subject>> GetSubject(int id)
		{
			var subject = await _context.Subjects
				.Where(s => s.Id == id)
				.Select(s => new Subject
				{
					Id = s.Id,
					Code = s.Code,
					Description = s.Description
				})
				.FirstOrDefaultAsync();

			if (subject == null)
				return NotFound();

			return subject;
		}

		[HttpPost]
		public async Task<ActionResult<Subject>> CreateSubject(Subject subjectDto)
		{
			var subject = new Subject
			{
				Code = subjectDto.Code,
				Description = subjectDto.Description
			};

			_context.Subjects.Add(subject);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetSubject), new { id = subject.Id }, subject);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateSubject(int id, Subject subjectDto)
		{
			if (id != subjectDto.Id)
				return BadRequest();

			var subject = await _context.Subjects.FindAsync(id);
			if (subject == null)
				return NotFound();

			subject.Code = subjectDto.Code;
			subject.Description = subjectDto.Description;

			_context.Entry(subject).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSubject(int id)
		{
			var subject = await _context.Subjects.FindAsync(id);
			if (subject == null)
				return NotFound();

			_context.Subjects.Remove(subject);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
