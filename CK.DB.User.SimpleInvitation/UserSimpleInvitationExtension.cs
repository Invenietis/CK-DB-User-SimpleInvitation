using System;

namespace CK.DB.User.SimpleInvitation
{
    /// <summary>
    /// Extends <see cref="IUserSimpleInvitationInfo"/>.
    /// </summary>
    public static class UserSimpleInvitationExtension
    {
        /// <summary>
        /// Checks whether this <see cref="IUserSimpleInvitationInfo"/> is not null,
        /// its <see cref="IUserSimpleInvitationInfo.InvitationId"/> is not zero and
        /// its <see cref="IUserSimpleInvitationInfo.ExpirationDateUtc"/> is greater than now.
        /// </summary>
        /// <param name="this">This <see cref="IUserSimpleInvitationInfo"/>.</param>
        /// <param name="allowedDelta">Optional timespan that applies to <see cref="DateTime.UtcNow"/>.</param>
        /// <returns>True if this invitation is valid.</returns>
        public static bool IsValid( this IUserSimpleInvitationInfo @this, TimeSpan? allowedDelta = null )
        {
            return @this != null
                   && @this.InvitationId > 0
                   && @this.ExpirationDateUtc > DateTime.UtcNow.Add( allowedDelta ?? TimeSpan.Zero );
        }
    }
}
