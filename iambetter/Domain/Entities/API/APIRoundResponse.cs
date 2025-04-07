using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iambetter.Domain.Entities.Models;

namespace iambetter.Domain.Entities.API
{
    public class APIRoundResponse
    {
        public List<FixtureResponse> Response { get; set; }

        public class Fixture
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Timezone { get; set; }
            public Venue Venue { get; set; }
            public Status Status { get; set; }
        }

        public class Status
        {
            public string Long { get; set; }
            public string Short { get; set; }
        }   
    }   
}