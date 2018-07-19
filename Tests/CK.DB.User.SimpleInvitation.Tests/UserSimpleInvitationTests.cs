using CK.Core;
using CK.SqlServer;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CK.Testing.DBSetupTestHelper;

namespace CK.DB.User.SimpleInvitation.Tests
{
    [TestFixture]
    public class UserSimpleInvitationTests
    {

        [Test]
        public void creating_starting_and_destroying_an_invitation()
        {
            var inv = TestHelper.StObjMap.Default.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                var email = Guid.NewGuid().ToString( "N" ) + "@abc.com";
                var exp = DateTime.UtcNow.AddHours( 1 );
                var r = inv.Create( ctx, 1, 1, email, exp );
                r.Success.Should().BeTrue();
                r.InvitationId.Should().BeGreaterThan( 0 );
                r.InvitationToken.Should().NotBeEmpty();

                var rs = inv.StartResponse( ctx, 1, r.InvitationToken );
                rs.InvitationId.Should().Be( r.InvitationId );
                rs.Options.Should().BeNull();

                inv.Destroy( ctx, 1, r.InvitationId );
            }
        }

        [Test]
        public void creating_an_invitation_to_an_existing_mail_fails()
        {
            var inv = TestHelper.StObjMap.Default.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                var email = Guid.NewGuid().ToString( "N" ) + "@abc.com";
                var exp = DateTime.UtcNow.AddHours( 1 );
                var r1 = inv.Create( ctx, 1, 1, email, exp );
                r1.Success.Should().BeTrue();

                var r2 = inv.Create( ctx, 1, 1, email, exp );
                r2.Success.Should().BeFalse();
            }
        }

        [Test]
        public void unexisting_invitation_token_does_not_start_an_invitation()
        {
            var inv = TestHelper.StObjMap.Default.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                string token = $"3712.{Guid.NewGuid().ToString()}";
                var rs = inv.StartResponse( ctx, 1, token );
                rs.Success.Should().BeFalse();
            }
        }

        [Test]
        public void invalid_invitation_token_raises_an_exception()
        {
            var inv = TestHelper.StObjMap.Default.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                string token = $"This is not.A valid token";
                inv.Invoking( sut => sut.StartResponse( ctx, 1, token ) ).Should().Throw<SqlDetailedException>();
            }
        }


    }
}
