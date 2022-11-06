SELECT * FROM transfer

SELECT transfer.transfer_id, transfer_type.transfer_type_desc AS [transfer Type], transfer_status.transfer_status_desc AS [transfer status],transfer.account_from, transfer.account_to, transfer.amount
FROM transfer INNER JOIN
transfer_status ON transfer.transfer_status_id = transfer_status.transfer_status_id INNER JOIN
transfer_type ON transfer.transfer_type_id = transfer_type.transfer_type_id
 

SELECT tenmo_user.user_id, tenmo_user.username, account.account_id, account.balance
FROM tenmo_user,account
WHERE tenmo_user.user_id = account.user_id;

SELECT * FROM transfer_status

SELECT * FROM transfer_type
