namespace Ada.Core.ViewModel.API
{
  public  class RequestResult
    {
        public RequestResult()
        {
            IsSuccess = false;
        }
        public int AddCount { get; set; }
        public int UpdateCount { get; set; }
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public int? CommentCount { get; set; }
        public int? LikeCount { get; set; }
        public int? ViewCount { get; set; }
       

    }
}
