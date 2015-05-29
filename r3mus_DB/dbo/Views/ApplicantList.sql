




CREATE VIEW [dbo].[ApplicantList]
AS
	SELECT 
		[Applicants].[Id],
		[Applicants].[Name],
		[Applicants].[EmailAddress],
		[Applicants].[ApiKey],
		[Applicants].[VerificationCode],
		[Applicants].[Information],
		[Applicants].[Age],
		[Applicants].[ToonAge],
		[Applicants].[Source],
		[Applied].[DateTimeCreated] AS [Applied],
		[Applications].[DateTimeCreated] AS [LastStatusUpdate],
		[Applications].[Status],
		[Applications].[DateTimeCreated],
		[Applications].[Notes],
		ISNULL([Reviewer].[UserName], '') AS [UserName]
	FROM [r3mus_DB].[dbo].[Applicants] AS [Applicants] WITH (NOLOCK)	
	INNER JOIN [r3mus_DB].[dbo].[Applications] AS [Applications] WITH (NOLOCK)
		ON [Applicants].[Id] = [Applications].[ApplicantId]
		AND [Applications].[DateTimeCreated] = 
		(
			SELECT MAX([DateTimeCreated])
			FROM [r3mus_DB].[dbo].[Applications]
			WHERE [Applicants].[Id] = [Applications].[ApplicantId]
		)		
	INNER JOIN [r3mus_DB].[dbo].[Applications] AS [Applied] WITH (NOLOCK)
		ON [Applied].[Id] = [Applied].[ApplicantId]
		AND [Applied].[DateTimeCreated] = 
		(
			SELECT MIN([DateTimeCreated])
			FROM [r3mus_DB].[dbo].[Applications]
			WHERE [Applicants].[Id] = [Applications].[ApplicantId]
		)
	LEFT OUTER JOIN [r3mus_DB].[dbo].[AspNetUsers] AS [Reviewer] WITH (NOLOCK)
		ON [Reviewer].[Id] = [Applications].[Reviewer_Id]