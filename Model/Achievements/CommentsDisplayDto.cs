namespace HRMS_Backend.Model.Achievements
{
    public class CommentsDisplayDto
    {
         public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<CommentsDisplayDto> Replies { get; set; } = new();
    }
}
