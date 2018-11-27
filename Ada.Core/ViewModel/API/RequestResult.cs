using System;

namespace Ada.Core.ViewModel.API
{
  public  class RequestResult
    {
        public RequestResult()
        {
            IsSuccess = false;
            UpdateCount = 0;
            AddCount = 0;
        }
        public int AddCount { get; set; }
        public int UpdateCount { get; set; }
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public int? CommentCount { get; set; }
        public int? LikeCount { get; set; }
        public int? ViewCount { get; set; }
        public DateTime? RequestTime { get; set; }

    }
}
