/***
 * AuthRepository.cs
 * 
 * 認証関連のリポジトリクラスを定義するファイルです。
 * 以下の処理を行います。
 * ・ユーザー登録
 * ・ログイン
 * ・ユーザー情報変更(パスワード変更)
 * ・ユーザー削除
 */

using System.Net.Http;
using System.Net.Http.Json;
namespace Collatia_MAUI.Data.Repositories.Auth
{
	public interface IAuthRepository
	{
		Task<User> Register(User user, string password, bool isAdmin = false);
		Task<User> LoginAsync(string username, string password);
		// Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);	// パスワード変更
		// Task<bool> DeleteUserAsync(string username, string password);							// ユーザー削除
		// Task<bool> UserExists(string username);
	}

	public class AuthRepository : IAuthRepository
	{
		private readonly HttpClient _httpClient;

		public AuthRepository(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		/// <summary>
		/// ユーザー登録処理メソッド
		/// </summary>
		/// <param name="username">ユーザー名</param>
		/// <param name="password">パスワード</param>
		/// <param name="isAdmin">管理者権限フラグ</param>
		/// <returns>登録されたユーザー</returns>
		public async Task<User> Register(User user, string password, bool isAdmin = false)
		{
			var requestData = new { user.Username, user.Password, user.IsAdmin };
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/register", requestData);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("ユーザー登録に失敗しました。");
			}

			return await response.Content.ReadFromJsonAsync<User>();
		}

		public async Task<User> LoginAsync(string username, string password)
		{
			var requestData = new { Username = username, Password = password };
			var response = await _httpClient.PostAsJsonAsync("api/auth/login", requestData);

			if (!response.IsSuccessStatusCode) throw new Exception("ログインに失敗しました。");

			return await response.Content.ReadFromJsonAsync<User>();
		}



	}
}