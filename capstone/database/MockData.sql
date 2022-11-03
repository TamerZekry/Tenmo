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

INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (typeId,statusId, accountFrom, accountTo)
VALUES
(2, 2, 2002, 2001, 999),
(2, 2, 2001, 2002, 999),
(2, 2, 2003, 2001, 50);


SELECT transfer_id, account_from, account_to, transfer_type_desc, transfer_status_desc, amount FROM transfer
JOIN account ON account.user_id = 1001
JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id
JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id
WHERE transfer.account_from = account.account_id OR transfer.account_to = account.account_id;

select * from tenmo_user;
select * from account;
select * from transfer;
select * from transfer_type;
select * from transfer_status;


SELECT * FROM transfer
JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id
JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id
WHERE account_to = 2001 AND transfer.transfer_status_id = 1;

SELECT * FROM transfer
JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id
JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id
WHERE transfer_id = 3001;

SELECT * FROM transfer
JOIN account ON account.user_id = 1001
JOIN transfer_status ON transfer_status.transfer_status_id = transfer.transfer_status_id
JOIN transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id 
WHERE transfer.account_from = account.account_id OR transfer.account_to = account.account_id;
--SELECT * FROM account JOIN tenmo_user ON account.user_id = tenmo_user.user_id WHERE

BEGIN TRANSACTION;

SELECT account.account_id, account.user_id, account.balance FROM account JOIN tenmo_user on account.user_id = tenmo_user.user_id;
SELECT * FROM transfer;

UPDATE account SET balance = 1500 WHERE account_id = 2001;
UPDATE account SET balance = 500 WHERE account_id = 2002;

INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount)
OUTPUT inserted.*
VALUES (2, 2, 2002, 2001, 500);


SELECT * FROM account JOIN tenmo_user on account.user_id = tenmo_user.user_id;
SELECT * FROM transfer;

SELECT * FROM transfer
JOIN account ON transfer.account_to = account.account_id
JOIN tenmo_user ON tenmo_user.user_id = account.account_id



ROLLBACK;
SELECT * FROM transfer_type;