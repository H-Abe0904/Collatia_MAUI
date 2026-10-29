using System.Net;
using Collatia_MAUI.Data.Repositories.Auth;

namespace Collatia_MAUI.AuthServices
{
	/// <summary>
	/// 認証サービスのインターフェースと実装
	/// </summary>
	public interface IAuthService
	{
		bool IsAuthenticated { get; }
		Task<bool> LoginAsync(string username, string password);
		Task LogoutAsync();
	}

	/// <summary>
	/// 認証サービスの実装クラス
	/// </summary>
	public class AuthService : IAuthService
	{
		private readonly IAuthRepository _authRepository;
		private readonly CookieContainer _CookieContainer;
		private readonly Uri _baseUri = new Uri(DotNetEnv.Env.GetString("API_BASE_URL") ?? throw new InvalidOperationException("API_BASE_URL is not set in the environment variables."));
		private const string CookieStorageKey = "AuthCookies";
		private User? _currentUser;

		/// <summary>
		/// AuthServiceのコンストラクタ
		/// </summary>
		/// <param name="authRepository">認証リポジトリ</param>
		/// <param name="cookieContainer">Cookieコンテナ</param>
		public AuthService(IAuthRepository authRepository, CookieContainer cookieContainer)
		{
			_authRepository = authRepository;
			_CookieContainer = cookieContainer;
		}

		public bool IsAuthenticated => _currentUser != null;

		/// <summary>
		/// ログイン処理を行うメソッド
		/// </summary>
		/// <param name="username">ユーザー名</param>
		/// <param name="password">パスワード</param>
		/// <returns>ログイン成功</returns>
		public async Task<bool> LoginAsync(string username, string password)
		{
			var user = await _authRepository.LoginAsync(username, password);
			if (user != null)
			{
				_currentUser = user;
				string? cookieHeader = _CookieContainer.GetCookies(_baseUri).ToString();
				await SecureStorage.SetAsync(CookieStorageKey, cookieHeader);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Cookieを復元するメソッド（App.xaml.cs などから呼び出す）
		/// </summary>
		/// <returns></returns>
		public async Task RestoreCookiesAsync()
		{
			try
			{
				var cookieHeader = await SecureStorage.GetAsync(CookieStorageKey);
				if (!string.IsNullOrEmpty(cookieHeader))
				{
					_CookieContainer.SetCookies(_baseUri, cookieHeader);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error restoring cookies: {ex.Message}");
			}
		}

		/// <summary>
		/// ログアウト処理を行うメソッド
		/// </summary>
		/// <returns></returns>
		public async Task LogoutAsync()
		{
			_currentUser = null;
			// Optionally, you can also clear any stored tokens or session data here.
			await Task.CompletedTask;
		}
	}
}