attach "openABAP.s3db" as openABAP;

create table if not exists DD01(
	TABLENAME    varchar(20) not null,
	DESCRIPTION  varchar(35) not null,
	PRIMARY key(tablename)
	);

create table if not exists DD02(
	TABLENAME    varchar(20) not null,
	FIELDNAME    varchar(20) not null,
	TYPE         varchar(4)  not null,
	LENGTH       int         not null,
	DESCRIPTION  varchar(35) not null,
	primary key(tablename, fieldname)
	);

.quit