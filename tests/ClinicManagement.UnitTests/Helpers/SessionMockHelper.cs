using System;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ClinicManagement.Web.Tests.Helpers
{
    /// <summary>
    /// Helper for setting up ISession mocks to work with ASP.NET Core's GetInt32/SetInt32 extension methods.
    /// ASP.NET Core's SessionExtensions.GetInt32 stores integers as 4 big-endian bytes.
    /// </summary>
    public static class SessionMockHelper
    {
        /// <summary>
        /// Encodes an integer value the same way ASP.NET Core's SetInt32 does (big-endian 4 bytes).
        /// </summary>
        public static byte[] EncodeInt32(int value)
        {
            return new byte[]
            {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value
            };
        }

        /// <summary>
        /// Sets up the session mock to return the given integer for the given key.
        /// </summary>
        public static void SetupInt32(Mock<ISession> sessionMock, string key, int value)
        {
            var bytes = EncodeInt32(value);
            sessionMock.Setup(s => s.TryGetValue(key, out bytes!)).Returns(true);
        }

        /// <summary>
        /// Sets up the session mock to return null (key not found) for the given key.
        /// </summary>
        public static void SetupMissingKey(Mock<ISession> sessionMock, string key)
        {
            byte[]? nullBytes = null;
            sessionMock.Setup(s => s.TryGetValue(key, out nullBytes!)).Returns(false);
        }
    }
}
