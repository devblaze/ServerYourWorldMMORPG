//using ServerYourWorldMMORPG.Models.Game.User;
//using System.Collections.Concurrent;
//using System.Security.Cryptography;

//namespace ServerYourWorldMMORPG.Services.Network
//{
//	public class SessionManagmentService
//	{
//		private ConcurrentDictionary<string, UserSession> _activeSessions;

//		public SessionManagmentService()
//		{
//			_activeSessions = new ConcurrentDictionary<string, UserSession>();
//		}

//		public string CreateSession(User user)
//		{
//			var sessionId = GenerateNewSessionId();
//			var userSession = new UserSession
//			{
//				User = user,
//				SessionId = sessionId,
//				LastActivity = DateTime.UtcNow
//				// ... other session-related data
//			};

//			_activeSessions.TryAdd(sessionId, userSession);
//			return sessionId;
//		}

//		public void TerminateSession(string sessionId)
//		{
//			_activeSessions.TryRemove(sessionId, out var removedSession);
//			// ... perform any cleanup necessary for the removed session
//		}
//		public void ExpireSessions()
//		{
//			var expiredSessions = _activeSessions.Where(pair =>
//				(DateTime.UtcNow - pair.Value.LastActivity) > TimeSpan.FromMinutes(SessionTimeoutMinutes))
//				.Select(pair => pair.Key);

//			foreach (var sessionId in expiredSessions)
//			{
//				TerminateSession(sessionId);
//			}
//		}

//		public bool ValidateSession(string sessionId)
//		{
//			if (_activeSessions.TryGetValue(sessionId, out var session))
//			{
//				// Optionally update session activity timestamp
//				session.LastActivity = DateTime.UtcNow;
//				return true;
//			}
//			return false;
//		}

//		private string GenerateNewSessionId()
//		{
//			// Use a secure method of generating unique session identifiers
//			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
//		}
//	}
//}
