/*
    Optimizes dashboard stored procedures for DataDepot.
    Root cause: original procs scanned large tables multiple times and exhausted SQL memory
    when the dashboard fired 5 parallel API calls.

    Run against CMSMeterLive (or your target database).
*/
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[PROC_Dashboard_CompVsPendingChart]
(
    @projectId bigint,
    @FormTypeId bigint
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Ids TABLE (Id bigint PRIMARY KEY);
    INSERT INTO @Ids (Id)
    SELECT Id
    FROM UploadedExcelData WITH (NOLOCK)
    WHERE FKProjectId = @projectId AND Active = 1;

    DECLARE @TotalRecords bigint = (SELECT COUNT(*) FROM @Ids);
    DECLARE @MappedRecords bigint = (
        SELECT COUNT(*)
        FROM tblInstallerMapData IM WITH (NOLOCK)
        INNER JOIN @Ids i ON i.Id = IM.FK_UploadedExcelData_Id
        WHERE IM.Active = 1
    );
    DECLARE @CompletedRecords bigint = (
        SELECT COUNT(DISTINCT F.ID)
        FROM @Ids i
        INNER JOIN tblInstallerMapData IM WITH (NOLOCK)
            ON i.Id = IM.FK_UploadedExcelData_Id AND IM.Active = 1
        INNER JOIN TblProjectFieldData F WITH (NOLOCK)
            ON IM.FK_UploadedExcelData_Id = F.FK_UploadedExcelData_Id
            AND IM.InstallerId = F.InstallerId AND F.Active = 1
        INNER JOIN tblProjectFieldDataMaster d WITH (NOLOCK)
            ON F.ID = d.TblProjectFieldData_Id AND d.Active = 1
            AND d.FieldValue = 'true' AND d.InitialFieldName = 'WORK COMPLETED'
    );
    DECLARE @SkippedRecords bigint = (
        SELECT COUNT(DISTINCT F.ID)
        FROM @Ids i
        INNER JOIN TblProjectFieldData F WITH (NOLOCK)
            ON i.Id = F.FK_UploadedExcelData_Id AND F.Active = 1 AND F.IsSkipped = 1
        INNER JOIN tblSkipReasonMaster SR WITH (NOLOCK)
            ON F.SkipId = SR.SkipId AND SR.CategoryType = 'SKIP' AND SR.Active = 1
        INNER JOIN tblFormMaster F1 WITH (NOLOCK) ON F.FormId = F1.FormId
        INNER JOIN tblFormType FT WITH (NOLOCK)
            ON F1.FormTypeID = FT.ID AND FT.ID = @FormTypeId AND FT.Active = 1
    );
    DECLARE @RtuRecords bigint = (
        SELECT COUNT(DISTINCT F.ID)
        FROM @Ids i
        INNER JOIN tblInstallerMapData IM WITH (NOLOCK)
            ON i.Id = IM.FK_UploadedExcelData_Id AND IM.Active = 1
        INNER JOIN TblProjectFieldData F WITH (NOLOCK)
            ON IM.FK_UploadedExcelData_Id = F.FK_UploadedExcelData_Id
            AND IM.InstallerId = F.InstallerId AND F.Active = 1 AND F.IsSkipped = 1
        INNER JOIN tblSkipReasonMaster SR WITH (NOLOCK)
            ON F.SkipId = SR.SkipId AND SR.CategoryType = 'RTU'
    );
    DECLARE @UnAllocatedSkip bigint = 0;

    IF (@projectId = 10085 OR @projectId = 10108)
    BEGIN
        SELECT @UnAllocatedSkip = COUNT(*)
        FROM @Ids i
        INNER JOIN TblUnAllocatedSkipHistory UN WITH (NOLOCK)
            ON i.Id = UN.FK_UploadedExcelData_Id AND UN.Active = 1
        WHERE NOT EXISTS (
            SELECT 1 FROM tblInstallerMapData IM WITH (NOLOCK)
            WHERE IM.FK_UploadedExcelData_Id = i.Id AND IM.Active = 1
        )
        AND NOT EXISTS (
            SELECT 1 FROM TblProjectFieldData F WITH (NOLOCK)
            WHERE F.FK_UploadedExcelData_Id = i.Id AND F.Active = 1
        );
    END

    SELECT
        P.ProjectId,
        P.ProjectName,
        P.ProjectStatus AS ProStatus,
        @TotalRecords AS TotalRecords,
        @MappedRecords AS MappedRecords,
        (@TotalRecords - @MappedRecords - @UnAllocatedSkip) AS UnMappedRecords,
        @CompletedRecords AS CompletedRecords,
        @SkippedRecords AS SkippedRecords,
        @RtuRecords AS RtuRecords,
        @UnAllocatedSkip AS UnAllocatedSkip
    FROM tblProject P WITH (NOLOCK)
    WHERE P.ProjectId = @projectId AND P.Active = 1;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[PROC_Dashboard_InstallationsPerInstaller]
(
    @projectId bigint,
    @FormTypeId bigint
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Ids TABLE (Id bigint PRIMARY KEY);
    INSERT INTO @Ids (Id)
    SELECT Id
    FROM UploadedExcelData WITH (NOLOCK)
    WHERE FKProjectId = @projectId AND Active = 1;

    DECLARE @TotalRecords bigint = (SELECT COUNT(*) FROM @Ids);

    ;WITH InstallerMap AS (
        SELECT IM.InstallerId, IM.FK_UploadedExcelData_Id, IM.ID AS MapId
        FROM tblInstallerMapData IM WITH (NOLOCK)
        INNER JOIN @Ids i ON i.Id = IM.FK_UploadedExcelData_Id
        WHERE IM.Active = 1
    ),
    Summary AS (
        SELECT
            IM.InstallerId,
            COUNT(DISTINCT IM.MapId) AS TotalMappedRecords,
            COUNT(DISTINCT CASE WHEN d.ID IS NOT NULL THEN F.ID END) AS CompletedRecords,
            COUNT(DISTINCT CASE WHEN F.IsSkipped = 1 AND SR.CategoryType = 'SKIP' AND FT.ID = @FormTypeId THEN F.ID END) AS SkippedRecords,
            COUNT(DISTINCT CASE WHEN F.IsSkipped = 1 AND SR.CategoryType = 'RTU' THEN F.ID END) AS RTURecords
        FROM InstallerMap IM
        LEFT JOIN TblProjectFieldData F WITH (NOLOCK)
            ON IM.FK_UploadedExcelData_Id = F.FK_UploadedExcelData_Id
            AND IM.InstallerId = F.InstallerId AND F.Active = 1
        LEFT JOIN tblProjectFieldDataMaster d WITH (NOLOCK)
            ON F.ID = d.TblProjectFieldData_Id AND d.Active = 1
            AND d.FieldValue = 'true' AND d.InitialFieldName = 'WORK COMPLETED'
        LEFT JOIN tblSkipReasonMaster SR WITH (NOLOCK) ON F.SkipId = SR.SkipId
        LEFT JOIN tblFormMaster F1 WITH (NOLOCK) ON F.FormId = F1.FormId
        LEFT JOIN tblFormType FT WITH (NOLOCK) ON F1.FormTypeID = FT.ID AND FT.Active = 1
        GROUP BY IM.InstallerId
    )
    SELECT
        S.InstallerId,
        ISNULL(U.FirstName, '') + ' ' + ISNULL(U.LastName, '') AS InstallerName,
        S.TotalMappedRecords,
        @TotalRecords AS TotalRecords,
        ISNULL(S.SkippedRecords, 0) AS SkippedRecords,
        ISNULL(S.RTURecords, 0) AS RTURecords,
        ISNULL(S.CompletedRecords, 0) AS CompletedRecords,
        (S.TotalMappedRecords - ISNULL(S.CompletedRecords, 0) - ISNULL(S.SkippedRecords, 0) - ISNULL(S.RTURecords, 0)) AS PendingRecords
    FROM Summary S
    LEFT JOIN tblUsers U WITH (NOLOCK) ON S.InstallerId = U.UserID;
END
GO

/*
    Optimizes STP_ProjectMonthSummary.
    Original proc used tblUploadedData + NOT IN and scanned TblProjectFieldData repeatedly.
    Newer projects (e.g. 10134) store rows in UploadedExcelData, so tblUploadedData returns 0
    and the old query still times out under parallel dashboard load.
*/
CREATE OR ALTER PROCEDURE [dbo].[STP_ProjectMonthSummary]
(
    @projectId bigint
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @projectStartDate datetime;
    DECLARE @projectEndDate datetime;
    DECLARE @TotalRecord bigint = 0;
    DECLARE @UnMappedTotalRecord bigint = 0;

    SELECT @projectStartDate = FromDate, @projectEndDate = ToDate
    FROM tblProject WITH (NOLOCK)
    WHERE ProjectId = @projectId AND Active = 1;

    IF @projectStartDate IS NULL OR @projectEndDate IS NULL
        RETURN;

    DECLARE @Ids TABLE (Id bigint PRIMARY KEY);

    INSERT INTO @Ids (Id)
    SELECT Id
    FROM UploadedExcelData WITH (NOLOCK)
    WHERE FKProjectId = @projectId AND Active = 1;

    IF NOT EXISTS (SELECT 1 FROM @Ids)
    BEGIN
        INSERT INTO @Ids (Id)
        SELECT ID
        FROM tblUploadedData WITH (NOLOCK)
        WHERE ProjectId = @projectId AND Active = 1;
    END

    SELECT @TotalRecord = COUNT(*) FROM @Ids;

    SELECT @UnMappedTotalRecord = COUNT(*)
    FROM @Ids i
    WHERE NOT EXISTS (
        SELECT 1
        FROM tblInstallerMapData IM WITH (NOLOCK)
        WHERE IM.Active = 1
          AND (
                IM.FK_UploadedExcelData_Id = i.Id
             OR IM.tblUploadedData_ID = i.Id
          )
    );

    ;WITH Months AS (
        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT 0)) AS MonthId,
            DATENAME(MONTH, DATEADD(MONTH, nos.monthnos - 1, @projectStartDate)) AS VisitMonth
        FROM (
            SELECT 1 AS monthnos UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
            UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8
            UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
        ) nos
        WHERE nos.monthnos <= DATEDIFF(MONTH, @projectStartDate, @projectEndDate) + 1
    ),
    Completed AS (
        SELECT
            DATENAME(MONTH, F.CreatedOn) AS VisitMonth,
            COUNT(*) AS CompletedRecords
        FROM @Ids i
        INNER JOIN TblProjectFieldData F WITH (NOLOCK)
            ON F.Active = 1
           AND (
                F.FK_UploadedExcelData_Id = i.Id
             OR F.tblUploadedData_Id = i.Id
           )
        INNER JOIN tblProjectFieldDataMaster d WITH (NOLOCK)
            ON F.ID = d.TblProjectFieldData_Id
           AND d.Active = 1
           AND d.FieldValue = 'true'
           AND d.InitialFieldName = 'WORK COMPLETED'
        GROUP BY DATENAME(MONTH, F.CreatedOn)
    ),
    Skipped AS (
        SELECT
            DATENAME(MONTH, F.CreatedOn) AS VisitMonth,
            COUNT(*) AS SkippedRecords
        FROM @Ids i
        INNER JOIN TblProjectFieldData F WITH (NOLOCK)
            ON F.Active = 1
           AND F.IsSkipped = 1
           AND (
                F.FK_UploadedExcelData_Id = i.Id
             OR F.tblUploadedData_Id = i.Id
           )
        GROUP BY DATENAME(MONTH, F.CreatedOn)
    )
    SELECT
        M.MonthId,
        M.VisitMonth,
        @TotalRecord AS TotalRecords,
        ISNULL(S.SkippedRecords, 0) AS SkippedRecords,
        ISNULL(C.CompletedRecords, 0) AS CompletedRecords,
        CAST(NULL AS bigint) AS PendingRecords,
        @UnMappedTotalRecord AS NotMappedRecords
    FROM Months M
    LEFT JOIN Completed C ON C.VisitMonth = M.VisitMonth
    LEFT JOIN Skipped S ON S.VisitMonth = M.VisitMonth
    ORDER BY M.MonthId;
END
GO
