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