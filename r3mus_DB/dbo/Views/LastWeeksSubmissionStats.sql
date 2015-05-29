


CREATE VIEW [dbo].[LastWeeksSubmissionStats]
AS
	SELECT DISTINCT
		RANK() OVER (ORDER BY COUNT ([Name]) ASC) AS [Id],
		[Submitter],
		(
			COUNT ([Name])
		) 
		AS [Submitted]
	FROM
	(
		SELECT [MAILEES].[Name], [SUB].[UserName] AS [Submitter]
		FROM [dbo].[RecruitmentMailees] AS [MAILEES] WITH (NOLOCK)
		INNER JOIN [dbo].[AspNetUsers] AS [SUB] WITH (NOLOCK)
			ON [SUB].[Id] = [MAILEES].[SubmitterId]
		WHERE [MAILEES].[Submitted] >= DATEADD(DD, -7, GETDATE())
		--AND [MAILEES].[CorpId_AtLastCheck] BETWEEN 1000000 AND 1000200
	) AS [a]
	GROUP BY [Submitter]