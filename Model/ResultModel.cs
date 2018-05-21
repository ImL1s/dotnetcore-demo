using System.Collections.Generic;

namespace dotnetcore_demo.Model
{
    public class ResultModel
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public List<string> Errors { get; set; }

        public object Data { get; set; }
    }
}