using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sparin.Documents.Configuration
{
    public class DocumentDbOptions
    {

        [Required]
        public string DatabaseName { get; set; }

    }
}
