namespace Twitter.Dto
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string TwitterHandle { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
