CREATE TABLE cdc.[Outbox] (
    [ChangeId]     BIGINT             IDENTITY (1, 1) NOT NULL,
    [ActualLSN]    BINARY (10)        ,
    [EventType]    VARCHAR (500)      NULL,
    [EventTopic]   VARCHAR (500)      NULL,
	[EventBatchDate] DATETIMEOFFSET (7),
    [EventSentUTC] DATETIMEOFFSET (7) NULL,
    PRIMARY KEY CLUSTERED ([ChangeId] ASC)
);

