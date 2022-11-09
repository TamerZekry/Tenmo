USE tenmoTesting
--GO

CREATE TABLE transfer_type (
	transfer_type_id int IDENTITY(1,1) NOT NULL,
	transfer_type_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_type PRIMARY KEY (transfer_type_id)
)

CREATE TABLE transfer_status (
	transfer_status_id int IDENTITY(1,1) NOT NULL,
	transfer_status_desc varchar(10) NOT NULL,
	CONSTRAINT PK_transfer_status PRIMARY KEY (transfer_status_id)
)

CREATE TABLE tenmo_user (
	user_id int IDENTITY(1001,1) NOT NULL,
	username varchar(50) NOT NULL,
	password_hash varchar(200) NOT NULL,
	salt varchar(200) NOT NULL,
	CONSTRAINT PK_user PRIMARY KEY (user_id),
	CONSTRAINT UQ_username UNIQUE (username)
)

CREATE TABLE account (
	account_id int IDENTITY(2001,1) NOT NULL,
	user_id int NOT NULL,
	balance decimal(13, 2) NOT NULL,
	CONSTRAINT PK_account PRIMARY KEY (account_id),
	CONSTRAINT FK_account_user FOREIGN KEY (user_id) REFERENCES tenmo_user (user_id)
)

CREATE TABLE transfer (
	transfer_id int IDENTITY(3001,1) NOT NULL,
	transfer_type_id int NOT NULL,
	transfer_status_id int NOT NULL,
	account_from int NOT NULL,
	account_to int NOT NULL,
	amount decimal(13, 2) NOT NULL,
	CONSTRAINT PK_transfer PRIMARY KEY (transfer_id),
	CONSTRAINT FK_transfer_account_from FOREIGN KEY (account_from) REFERENCES account (account_id),
	CONSTRAINT FK_transfer_account_to FOREIGN KEY (account_to) REFERENCES account (account_id),
	CONSTRAINT FK_transfer_transfer_status FOREIGN KEY (transfer_status_id) REFERENCES transfer_status (transfer_status_id),
	CONSTRAINT FK_transfer_transfer_type FOREIGN KEY (transfer_type_id) REFERENCES transfer_type (transfer_type_id),
	CONSTRAINT CK_transfer_not_same_account CHECK  ((account_from<>account_to)),
	CONSTRAINT CK_transfer_amount_gt_0 CHECK ((amount>0))
)


INSERT INTO transfer_status (transfer_status_desc) VALUES ('Pending');
INSERT INTO transfer_status (transfer_status_desc) VALUES ('Approved');
INSERT INTO transfer_status (transfer_status_desc) VALUES ('Rejected');

INSERT INTO transfer_type (transfer_type_desc) VALUES ('Request');
INSERT INTO transfer_type (transfer_type_desc) VALUES ('Send');


INSERT INTO tenmo_user (username, password_hash, salt)
VALUES
('mike', 'WadO0XbcB5UuS8MEMY/LUcvlzos=', 'wgAtx+Ant2Q='),
('tamer', '8IjJFu0pXaKuPgWymiYk9/o5BVw=', 'ikOV1mV0Ofw='),
('alex', 'pKiKVUS6XIQlgSv/WYVNC7tt6vo=', 'dT+xNXq2MOg='),
('colin', 'pKiKVUS6XIQlgSv/WYVNC7tt6vo=', 'dT+xNXq2MOg=');

INSERT INTO account (user_id, balance)
VALUES
(1001, 1000),
(1002, 1000),
(1003, 1000),
(1004, 1000);

INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount)
VALUES
(2, 2, 2002, 2001, 50),
(2, 2, 2001, 2002, 50),
(2, 2, 2003, 2001, 50),
(1, 1, 2002, 2001, 100);