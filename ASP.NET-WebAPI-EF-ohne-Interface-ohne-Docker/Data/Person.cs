using System;
using System.Collections.Generic;

namespace PersonApi.Data
{
    public partial class Person
    {
        public int PersonId { get; set; }
        public string? Vorname { get; set; }
        public string? Nachname { get; set; }
        public string? Email { get; set; }
        public DateTime? GebDatum { get; set; }
    }
}
