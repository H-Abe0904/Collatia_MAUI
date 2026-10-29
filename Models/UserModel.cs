/**
 * ユーザーモデル
 * 
 * 作成者: H-Abe
 * 作成日: 2026/10/29
 * 
 * 更新履歴:
 * 2026/10/29 - 初版作成
 */

/// <summary>
/// ユーザーモデル
/// </summary>
public class User
{
	public int Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public bool IsLogin { get; set; } // ログイン状態フラグ
	public bool IsAdmin { get; set; } // 管理者権限フラグ
}