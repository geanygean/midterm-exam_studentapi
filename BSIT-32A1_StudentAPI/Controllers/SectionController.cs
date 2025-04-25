using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SectionsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public SectionsController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<object>>> GetSections()
		{
			var sections = await _context.Sections
				.Include(s => s.Subject)
				.Select(s => new
				{
					SectionId = s.Id,
					s.Name,
					s.SubjectId,
					Subject = new
					{
						s.Subject.Id,
						s.Subject.Code,
						s.Subject.Description
					},
					StudentSections = s.StudentSections.Select(ss => new
					{
						ss.StudentId,
						ss.SectionId
					})
				})
				.ToListAsync();

			return Ok(sections);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Section>> GetSection(int id)
		{
			var section = await _context.Sections
				.Include(s => s.Subject)
				.FirstOrDefaultAsync(s => s.Id == id);

			if (section == null)
				return NotFound();

			return Ok(section);
		}

		[HttpPost]
		public async Task<ActionResult<Section>> CreateSection(Section section)
		{
			_context.Sections.Add(section);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetSection), new { id = section.Id }, section);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateSection(int id, Section section)
		{
			if (id != section.Id) return BadRequest();

			var existing = await _context.Sections.FindAsync(id);
			if (existing == null) return NotFound();

			existing.Name = section.Name;
			existing.SubjectId = section.SubjectId;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteSection(int id)
		{
			var section = await _context.Sections.FindAsync(id);
			if (section == null)
				return NotFound();

			_context.Sections.Remove(section);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
