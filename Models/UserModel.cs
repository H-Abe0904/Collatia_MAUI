public class User
{
	public int Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public bool IsLogin { get; set; } // ログイン状態フラグ
	public bool IsAdmin { get; set; } // 管理者権限フラグ
}