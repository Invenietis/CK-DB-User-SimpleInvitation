
create table CK.tUserSimpleInvitation
(
	InvitationId int not null identity(0, 1),
	CreatedById int not null,
    EMail nvarchar(255) collate Latin1_General_100_CI_AS not null, 
	ExpirationDateUtc datetime2(2) not null,
	TokenGuid uniqueidentifier not null constraint DF_CK_UserSimpleInvitation_TokenGuid default( newid() ),
	InvitationSendCount int not null,
	LastInvitationSendDate datetime2(2) not null,
    ResponseStartedDate datetime2(2) not null constraint DF_CK_UserSimpleInvitation_ResponseStartedDate default( '0001-01-01' ),
    InvitationToken as cast(InvitationId as varchar(30)) + '.' + cast(TokenGuid as varchar(40)),
    Options varbinary(max) null,

	constraint PK_CK_UserSimpleInvitation primary key( InvitationId ),
	constraint UK_CK_UserSimpleInvitation_EMail unique( EMail ),
	constraint FK_CK_UserSimpleInvitation_CreatedById foreign key( CreatedById ) references CK.tUser( UserId )

);

insert into CK.tUserSimpleInvitation(CreatedById, EMail, ExpirationDateUtc, TokenGuid, InvitationSendCount, LastInvitationSendDate)
    values( 0, N'', '0001-01-01', '00000000-0000-0000-0000-000000000000', 0, '0001-01-01' );
