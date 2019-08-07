﻿

CREATE PROCEDURE [cdc].[StoreOutboxPostmark]
	@postmarks cdc.OutboxPostmarkType READONLY
AS


/*CREATE TABLE cdc.[OutboxPostmarks] (
    [ChangeId]     BIGINT             IDENTITY (1, 1) NOT NULL,
    [ActualLSN]    BINARY (10)        NULL,
    [EventType]    VARCHAR (500)      NULL,
    [EventTopic]   VARCHAR (500)      NULL,
    [EventSentUTC] DATETIMEOFFSET (7) NULL,
    PRIMARY KEY CLUSTERED ([ChangeId] ASC)
);
*/

	INSERT INTO OutboxPostmarks ([ActualLSN], [EventType], [EventTopic], [EventSentUTC])
	SELECT StartLSN, [EventType], [EventTopic], [EventSentUTC] from @postmarks


RETURN 0
