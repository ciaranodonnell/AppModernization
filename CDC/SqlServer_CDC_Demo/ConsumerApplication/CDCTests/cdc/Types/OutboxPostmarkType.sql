CREATE TYPE cdc.OutboxPostmarkType AS TABLE
(
    StartLSN BINARY(10) NOT NULL,
	[EventType]    VARCHAR (500)      NULL,
    [EventTopic]   VARCHAR (500)      NULL,
    [EventSentUTC] DATETIMEOFFSET (7) NULL
);