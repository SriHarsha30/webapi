CREATE proc InsertIntoNotificcation1
	@sendersId varchar(20),
	@receiversId varchar(20),
	@notification_Descpirtion varchar(250)
as
begin
	insert into notifications1 (sendersId,receiversId, CreatedDate, notification_Descpirtion) values(@sendersId, @receiversId, GETDATE(), @notification_Descpirtion);
	print 'inserted message to notification';
end