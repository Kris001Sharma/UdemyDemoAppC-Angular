using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.API.Controllers
{
    [Route("api/[controller]")]
    // Equivalent to ~ http://localhost:5000/api/Values
    // [controller] is a place holder for the class name ie. Values 
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // To access data from DB, inject Data Context using constuctor
        private readonly DataContext _context; // option 2 : DataContext context
        // to enable "context" to be accessed throughout ValuesController class
        // initialize field from parameter
        public ValuesController(DataContext context)
        {
            _context = context; // option 2 : this.context = context
        }

        /** 1.Synchronous => Thread would be blocked until call made to the DB and data returned **/
        // public IActionResult Get()                           
        // {
        //     var values = _context.Values.ToList();
        //     return Ok(values);
        // }

        // public IActionResult Get(int id)
        // {
        //     var value = _context.Values.FirstOrDefault(x => x.Id == id);
        //     return Ok(value);
        // }        


        /** 2.Async: Thread kept open to handle requests, 
            passes the action to get data from DB to delegate
            when result returned it continues with request without blocking others **/
        [HttpGet]
        public async Task<IActionResult> Get()
        { // Task: represents async operation that can return a value
            // ie. keep thread open whilst waiting for response
            var values = await _context.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var values = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(values);
        }


        // POST api/values
        // Post : Create a record
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        // Put : Edit a Record
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        // Delete our record
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}