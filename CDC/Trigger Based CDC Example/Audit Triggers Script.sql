IF EXISTS (Select * from INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_NAME = 'AddAuditTable')
	DROP PROCEDURE AddAuditTable

GO

Create Procedure AddAuditTable 
(
	@TableName NVARCHAR(200),
	@AuditSchema NVARCHAR(200) = NULL,
	@OuputScripts BIT = 1,
	@GenerateScriptDontApply BIT = 0
)
AS
BEGIN

	DECLARE @ERRORTEXT VARCHAR(500)

	DECLARE @TheTableName NVARCHAR(100)
	DECLARE @TableSchema NVARCHAR(100)

	
	DECLARE @AuditTableName NVARCHAR(100)

	DECLARE @TempInt INT
	SELECT @TempInt = CHARINDEX('.', @TableName) 
	
	IF (@TempInt > 0)
	BEGIN
		SELECT @TheTableName= SUBSTRING(@TableName, @TempInt +1, (LEN(@TableName) - @TempInt))
		SELECT @TheTableName = REPLACE(@TheTableName, LEFT(QUOTENAME(''),1), '')
		SELECT @TheTableName = REPLACE(@TheTableName, RIGHT(QUOTENAME(''),1), '')
	

		SELECT @TableSchema = SUBSTRING(@TableName, 1, @TempInt - 1)
	END
	ELSE
	BEGIN
		SELECT @TheTableName = REPLACE(@TableName, LEFT(QUOTENAME(''),1), '')
		SELECT @TheTableName = REPLACE(@TheTableName, RIGHT(QUOTENAME(''),1), '')
		SELECT @TableSchema = 'dbo'

	END

	IF @GenerateScriptDontApply = 0 
	BEGIN
		IF NOT EXISTS (Select * from INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_NAME = @TheTableName AND t.TABLE_SCHEMA = @TableSchema)
		BEGIN
			SELECT @ERRORTEXT = 'The table you specified (' + @TheTableName + ') doesnt existing in schema ' + @TableSchema + '
			Switching @GenerateScriptDontApply to ON'
						
			SELECT @GenerateScriptDontApply = 1
			RAISERROR (@ERRORTEXT,16,1)
		END
	END

	IF (@AuditSchema  IS NULL)	SELECT @AuditSchema = @TableSchema
	SELECT @AuditTableName = @TheTableName + '_Audit'

	IF @GenerateScriptDontApply = 0 
	BEGIN
	
		IF  EXISTS (Select * from INFORMATION_SCHEMA.TABLES t WHERE t.TABLE_NAME = @AuditTableName AND t.TABLE_SCHEMA = @AuditSchema)
		BEGIN
			SELECT @ERRORTEXT = 'The audit table for table ' + @TheTableName + 'already exists in schema ' + @AuditSchema + '
			Switching @GenerateScriptDontApply to ON'
						
			SELECT @GenerateScriptDontApply = 1
			RAISERROR (@ERRORTEXT,16,1)
		END
	END

	-----------------CREATE TABLE SCRIPT ---------------------------------

	DECLARE @CreateTableSQL NVARCHAR(MAX)
	SELECT @CreateTableSQL = 
	'CREATE TABLE ' + @AuditSchema + '.' + QUOTENAME(@AuditTableName) + '
	(
	'
	

	SELECT @CreateTableSQL  = @CreateTableSQL + 
		COLUMN_NAME + ' ' + DATA_TYPE + 
		CASE WHEN DATA_TYPE = 'varchar' OR DATA_TYPE = 'char'  OR DATA_TYPE = 'nvarchar'  OR DATA_TYPE = 'nchar'  OR DATA_TYPE = 'varbinary' THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH as VARCHAR(10)) + ')' ELSE ''  END +
		CASE WHEN DATA_TYPE = 'decimal' OR DATA_TYPE = 'numeric'  THEN '(' + CAST(NUMERIC_PRECISION as VARCHAR(10)) + ',' + CAST(NUMERIC_SCALE as VARCHAR(10)) + ')' ELSE ''  END +
	  ',
	'
	 FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_SCHEMA=@TableSchema and TABLE_NAME= @TheTableName 

	
	SELECT @CreateTableSQL  =@CreateTableSQL  + 'Audit_Updated DATETIMEOFFSET(7),
	Audit_UpdatedBy varchar(100),
	Audit_UpdateType CHAR(1),
	Audit_UpdateNumber bigint IDENTITY(1,1) NOT NULL,
	CONSTRAINT [PK_' + @AuditTableName + '] PRIMARY KEY CLUSTERED 
		(
			[Audit_UpdateNumber] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
	
	
	
	
	
	'


	IF @OuputScripts = 1 OR @GenerateScriptDontApply = 1
		PRINT @CreateTableSQL



	------------------------ CREATE TRIGGER SCRIPT -----------------------------------

	
	
	DECLARE @TriggerColumnList NVARCHAR(MAX)

	SET @TriggerColumnList = '	'

	SELECT 
	@TriggerColumnList =
	 @TriggerColumnList + COLUMN_NAME + ',
	 '
	 FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_SCHEMA=@TableSchema and TABLE_NAME= @TheTableName 

	
	DECLARE @CreateTriggerSQL NVARCHAR(MAX)

	SELECT @CreateTriggerSQL = '
CREATE TRIGGER ' + @AuditSchema + '.CDCTrigger_' + @TheTableName  + '
   ON  ' + @TableSchema + '.' + @TheTableName  + '
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @UpdateType CHAR


	IF EXISTS (SELECT * from inserted)
	BEGIN
		IF EXISTS (SELECT * FROM deleted)
		BEGIN
			SET @UpdateType = ''U''
		END
		ELSE
		BEGIN
			SET @UpdateType = ''I''
		END

		INSERT INTO ' + @AuditSchema +'.' + @AuditTableName + '(
        ' + @TriggerColumnList + 'Audit_Updated,
	 Audit_UpdatedBy,
	 Audit_UpdateType
	
    )
    SELECT
        ' + @TriggerColumnList + '
        GETUTCDATE(),
		SUSER_NAME(),
		@UpdateType
       
    FROM
        inserted i

	END
	ELSE
	BEGIN
		SET @UpdateType = ''D''

		INSERT INTO ' + @AuditSchema +'.' + @AuditTableName + '(
        ' + @TriggerColumnList + ' 
        Audit_Updated,
	Audit_UpdatedBy,
	Audit_UpdateType
	
    )
    SELECT
        ' + @TriggerColumnList + '
        GETUTCDATE(),
		SUSER_NAME(),
		@UpdateType
        
    FROM
        deleted i
	END
END
	'

	IF @OuputScripts = 1 OR @GenerateScriptDontApply = 1
	BEGIN
		PRINT @CreateTriggerSQL 
	END

	IF @GenerateScriptDontApply = 1 
	BEGIN 
		PRINT '-- Scripts Generated, Not Applying'
	END
	ELSE
	BEGIN
		PRINT 'PRINT Going to run Create Table'
		exec sp_sqlexec @CreateTableSQL
		PRINT 'PRINT Done'
		PRINT 'PRINT Going to run Create Trigger'
		exec sp_sqlexec @CreateTriggerSQL 
		PRINT 'PRINT Done'
	END
	
	PRINT 'PRINT Complete'		
		
END

GO


EXEC AddAuditTable '[Customer]', 'dbo',1,1




/*


SELECT *
	 FROM INFORMATION_SCHEMA.COLUMNS

	 */
