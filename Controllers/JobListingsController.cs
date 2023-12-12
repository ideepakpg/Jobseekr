using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Jobseekr.Models;

namespace Jobseekr.Controllers
{
    public class JobListingsController : ApiController
    {
        private JobseekrDBContext db = new JobseekrDBContext();

        // GET: api/JobListings
        public IQueryable<JobListing> GetjobListings()
        {
            return db.jobListings;
        }

        // GET: api/JobListings/5
        [ResponseType(typeof(JobListing))]
        public IHttpActionResult GetJobListing(int id)
        {
            JobListing jobListing = db.jobListings.Find(id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return Ok(jobListing);
        }

        // PUT: api/JobListings/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutJobListing(int id, JobListing jobListing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != jobListing.JobId)
            {
                return BadRequest();
            }

            db.Entry(jobListing).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobListingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/JobListings
        [ResponseType(typeof(JobListing))]
        public IHttpActionResult PostJobListing(JobListing jobListing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.jobListings.Add(jobListing);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = jobListing.JobId }, jobListing);
        }

        // DELETE: api/JobListings/5
        [ResponseType(typeof(JobListing))]
        public IHttpActionResult DeleteJobListing(int id)
        {
            JobListing jobListing = db.jobListings.Find(id);
            if (jobListing == null)
            {
                return NotFound();
            }

            db.jobListings.Remove(jobListing);
            db.SaveChanges();

            return Ok(jobListing);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool JobListingExists(int id)
        {
            return db.jobListings.Count(e => e.JobId == id) > 0;
        }
    }
}