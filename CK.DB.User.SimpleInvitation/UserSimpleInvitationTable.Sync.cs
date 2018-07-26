using CK.Core;
using CK.Setup;
using CK.SqlServer;
using CK.SqlServer.Setup;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CK.DB.User.SimpleInvitation
{
    public abstract partial class UserSimpleInvitationTable
    {
        /// <summary>
        /// Creates a new invitation.
        /// </summary>
        /// <param name="ctx">The call context to use.</param>
        /// <param name="actorId">The current actor identifier (becomes the sender of the invitation).</param>
        /// <param name="info">Invitation information.</param>
        /// <param name="firstInvitationSent">True if an initial invitation mail has already been sent, prior to the invitation creation.</param>
        /// <returns>The <see cref="CreateResult"/> with the invitation identifier and token to use.</returns>
        [SqlProcedure( "sUserSimpleInvitationCreate" )]
        public abstract CreateResult Create( ISqlCallContext ctx, int actorId, [ParameterSource]IUserSimpleInvitationInfo info, bool firstInvitationSent = false );

        /// <summary>
        /// Starts a response to an invitation.
        /// </summary>
        /// <param name="ctx">The call context to use.</param>
        /// <param name="actorId">The current actor identifier.</param>
        /// <param name="invitationToken">The invitation token.</param>
        /// <returns>The <see cref="IUserSimpleInvitationInfo"/>.</returns>
        [SqlProcedure( "sUserSimpleInvitationStartResponse" )]
        public abstract IUserSimpleInvitationInfo StartResponse( ISqlCallContext ctx, int actorId, string invitationToken );

        /// <summary>
        /// Destroys an existing invitation either because it succeeded or it must be canceled.
        /// </summary>
        /// <param name="ctx">The call context to use.</param>
        /// <param name="actorId">The current actor identifier.</param>
        /// <param name="invitationId">The invitation identifier to destroy.</param>
        /// <param name="successful">True to indicate a successful registration. False to cancel it.</param>
        [SqlProcedure( "sUserSimpleInvitationDestroy" )]
        public abstract void Destroy( ISqlCallContext ctx, int actorId, int invitationId, bool successful = false );

    }
}
