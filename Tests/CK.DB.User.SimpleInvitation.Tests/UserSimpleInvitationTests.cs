using CK.Core;
using CK.SqlServer;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CK.Testing.MonitorTestHelper;

namespace CK.DB.User.SimpleInvitation.Tests
{
    [TestFixture]
    public class UserSimpleInvitationTests
    {

        [TestCase( true )]
        [TestCase( false )]
        public void creating_starting_and_destroying_an_invitation( bool firstInvitationSent )
        {
            var inv = SharedEngine.Map.StObjs.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                var info = inv.CreateInfo();
                info.EMail = Guid.NewGuid().ToString( "N" ) + "@abc.com";
                info.ExpirationDateUtc = DateTime.UtcNow.AddHours( 1 );
                var r = inv.Create( ctx, 1, info, firstInvitationSent );
                r.Success.Should().BeTrue();
                r.InvitationId.Should().BeGreaterThan( 0 );
                r.InvitationToken.Should().NotBeEmpty();
                var rs = inv.StartResponse( ctx, 1, r.InvitationToken );
                rs.IsValid().Should().BeTrue();
                rs.InvitationId.Should().Be( r.InvitationId );
                rs.Options.Should().BeNull();
                if( firstInvitationSent )
                {
                    rs.InvitationSendCount.Should().Be( 1 );
                    rs.LastInvitationSendDate.Should().BeCloseTo( DateTime.UtcNow, TimeSpan.FromMilliseconds( 500 ) );
                }
                else
                {
                    rs.InvitationSendCount.Should().Be( 0 );
                    rs.LastInvitationSendDate.Should().Be( Util.UtcMinValue );
                }
                inv.Destroy( ctx, 1, r.InvitationId );
            }
        }

        [Test]
        public void creating_an_invitation_to_an_existing_mail_fails()
        {
            var inv = SharedEngine.Map.StObjs.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                var info = inv.CreateInfo();
                info.EMail = Guid.NewGuid().ToString( "N" ) + "@abc.com";
                info.ExpirationDateUtc = DateTime.UtcNow.AddHours( 1 );
                var r1 = inv.Create( ctx, 1, info );
                r1.Success.Should().BeTrue();

                var r2 = inv.Create( ctx, 1, info );
                r2.Success.Should().BeFalse();
            }
        }

        [Test]
        public void unexisting_invitation_token_does_not_start_an_invitation()
        {
            var inv = SharedEngine.Map.StObjs.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                string token = $"3712.{Guid.NewGuid().ToString()}";
                var rs = inv.StartResponse( ctx, 1, token );
                rs.InvitationId.Should().Be( 0 );
                rs.ExpirationDateUtc.Should().Be( Util.UtcMinValue );
                rs.EMail.Should().BeNull();
                rs.InvitationToken.Should().Be( token );
                rs.Options.Should().BeNull();
            }
        }

        [Test]
        public void invalid_invitation_token_raises_an_exception()
        {
            var inv = SharedEngine.Map.StObjs.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                string token = $"This is not.A valid token";
                inv.Invoking( sut => sut.StartResponse( ctx, 1, token ) ).Should().Throw<SqlDetailedException>();
            }
        }

        [Test]
        public void invitation_expiration_is_boosted_by_at_least_5_minutes_when_invitation_starts()
        {
            var inv = SharedEngine.Map.StObjs.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                var info = inv.CreateInfo();
                info.EMail = Guid.NewGuid().ToString( "N" ) + "@abc.com";
                info.ExpirationDateUtc = DateTime.UtcNow.AddMinutes( 2 );
                var r1 = inv.Create( ctx, 1, info );
                r1.Success.Should().BeTrue();

                var boosted = info.ExpirationDateUtc.AddMinutes( 5 );
                var startInfo = inv.StartResponse( ctx, 1, r1.InvitationToken );
                startInfo.InvitationId.Should().BeGreaterThan( 0 );
                boosted.Should().BeBefore( startInfo.ExpirationDateUtc );
            }
        }

        [Test]
        public async Task invitation_expiration()
        {
            var inv = SharedEngine.Map.StObjs.Obtain<UserSimpleInvitationTable>();
            using( var ctx = new SqlStandardCallContext() )
            {
                var info = inv.CreateInfo();
                info.EMail = Guid.NewGuid().ToString( "N" ) + "@abc.com";
                info.ExpirationDateUtc = DateTime.UtcNow.AddMilliseconds( 500 );
                var r1 = await inv.CreateAsync( ctx, 1, info );
                r1.Success.Should().BeTrue();

                await Task.Delay( 550 );
                var startInfo = inv.StartResponse( ctx, 1, r1.InvitationToken );
                startInfo.IsValid().Should().BeFalse();
            }
        }


    }
}
