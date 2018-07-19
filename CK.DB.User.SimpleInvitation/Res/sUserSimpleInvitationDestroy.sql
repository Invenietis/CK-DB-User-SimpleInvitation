-- SetupConfig: {}
create procedure CK.sUserSimpleInvitationDestroy
(
	@ActorId int,
	@InvitationId int,
    @Successful bit = 0
)
as
begin
    if @ActorId <= 0 throw 50000, 'Security.AnonymousNotAllowed', 1;
    if @InvitationId <= 0 throw 50000, 'Argument.InvalidInvitationId', 1;

	--[beginsp]

    --<PreDestroy />

    delete from CK.tUserSimpleInvitation where InvitationId = @InvitationId;

    --<PostDestroy revert />
        
	--[endsp]
end

