using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace corel
{
    public class JobInfo
    {
        public int Id { set; get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public JOB_TYPE Type { set; get; }
    }
}
