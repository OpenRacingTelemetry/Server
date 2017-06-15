using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OpenRacingTelemetry.Data;
using OpenRacingTelemetry.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRacingTelemetry.Controllers
{
    [Route("api")]
    [Authorize]
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("races")]
        public IActionResult GetRaceById()
        {
            var ret = new List<Race>();
            foreach(var race in _context.Race)
            {
                ret.Add(race);
            }
            return Json(ret);
        }

        [HttpPost("races")]
        public IActionResult CreateRace([FromBody] Race race)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(v => v.Value.Errors.Select(e => e.Exception));

                List<string> messages = new List<string>();

                foreach (Exception e in errors)
                {
                    messages.Add(e.GetType().ToString() + ": " + e.Message);
                }

                return BadRequest(Json(messages));
            }


            if (race == null)
            {
                return BadRequest();
            }

            _context.Race.Add(race);
            _context.SaveChanges();

            return Json(race);
        }

        [HttpGet("races/{id}")]
        public IActionResult GetRaceById(int id)
        {
            var item = _context.Race.FirstOrDefault(t => t.RaceId == id);
            if (item == null)
            {
                return NotFound();
            }

            return Json(item);
        }

        [HttpDelete("races/{id}")]
        public IActionResult DeleteRaceById(int id)
        {
            var race = _context.Race.FirstOrDefault(t => t.RaceId == id);
            if (race == null)
            {
                return NotFound();
            }

            _context.Race.Remove(race);
            _context.SaveChanges();

            return new NoContentResult();
        }

        [HttpPut("races/{id}")]
        public IActionResult UpdateRaceById(int id, [FromBody] Race update)
        {
            if (update == null || update.RaceId != id)
            {
                return BadRequest();
            }

            var race = _context.Race.FirstOrDefault(t => t.RaceId == id);
            if (race == null)
            {
                return NotFound();
            }

            _context.Race.Update(race);
            _context.SaveChanges();

            return new NoContentResult();
        }


        [HttpGet("races/{id}/{recid}")]
        public IActionResult GetRecordById(int id,int recid)
        {
            var item = _context.Race.FirstOrDefault(t => t.RaceId == id);
            if (item == null)
            {
                return NotFound();
            }

            var rec = item.Records.FirstOrDefault(y => y.Id == recid);
            if (rec == null)
            {
                return NotFound();
            }

            return Json(rec);
        }

        [HttpDelete("races/{id}/{recid}")]
        public IActionResult DeleteRecordById(int id, int recid)
        {
            var item = _context.Race.FirstOrDefault(t => t.RaceId == id);
            if (item == null)
            {
                return NotFound();
            }

            var rec = item.Records.FirstOrDefault(y => y.Id == recid);
            if (rec == null)
            {
                return NotFound();
            }

            item.Records.Remove(rec);
            _context.Records.Remove(rec);
            _context.SaveChanges();

            return new NoContentResult();
        }

        [HttpPost("races/{id}")]
        public IActionResult CreateRecordById(int id, [FromBody] Record rec)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(v => v.Value.Errors.Select(e => e.Exception));

                List<string> messages = new List<string>();

                foreach (Exception e in errors)
                {
                    if (e != null)
                    {
                        messages.Add(e.GetType().ToString() + ": " + e.Message);
                    }
                }

                return BadRequest(Json(messages));
            }


            var item = _context.Race.FirstOrDefault(t => t.RaceId == id);
            if (item == null)
            {
                return NotFound();
            }

            if(item.Records==null)
            {
                item.Records = new List<Record>();
            }

            item.Records.Add(rec);
            _context.SaveChanges();

            return Json(rec);
        }

        [HttpPut("races/{id}/{recid}")]
        public IActionResult UpdateRecordById(int id, int recid, [FromBody] Record rec)
        {
            if (rec == null || rec.Id != recid)
            {
                return BadRequest();
            }

            var item = _context.Race.FirstOrDefault(t => t.RaceId == id);
            if (item == null)
            {
                return NotFound();
            }

            var recc = item.Records.FirstOrDefault(y => y.Id == recid);
            if (recc == null)
            {
                return NotFound();
            }

            _context.Records.Update(rec);
            _context.SaveChanges();
          
            return Json(rec);
        }
    }
}
