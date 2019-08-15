using CK.Core;
using System;

namespace CK.DB.User.SimpleInvitation
{
    public interface IUserSimpleInvitationInfo : IPoco
    {
        /// <summary>
        /// Gets the invitation identifier.
        /// This property can only be read from the database.
        /// </summary>
        int InvitationId { get; }

        /// <summary>
        /// Gets the invitation token to use.
        /// This property can only be read from the database.
        /// </summary>
        string InvitationToken { get; }

        /// <summary>
        /// The email of the invited user.
        /// This is the key for an invitation: only one invitation can exist by EMail.
        /// </summary>
        string EMail { get; set; }

        /// <summary>
        /// The expiration date. Must always be in the future.
        /// </summary>
        DateTime ExpirationDateUtc { get; set; }

        /// <summary>
        /// Gets the number of invitation sent so far.
        /// This property can only be read from the database.
        /// </summary>
        int InvitationSendCount { get; }

        /// <summary>
        /// Gets the last time (Utc) this invitation has been sent.
        /// Defaults to <see cref="Util.UtcMinValue"/>.
        /// This property can only be read from the database.
        /// </summary>
        DateTime LastInvitationSendDate { get; }

        /// <summary>
        /// Gets or sets an optional binary options that may
        /// contain invitation specific data
        /// </summary>
        byte[] Options { get; set; }

    }
}
