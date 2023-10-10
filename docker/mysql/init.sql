drop user  'root'@'%';

create user 'b101'@'%' identified by 'fjdjf384ohtfea838oa';
create user 'b101'@'localhost' identified by 'fjdjf384ohtfea838oa';

grant all privileges on *.* to 'b101'@'%';
grant all privileges on *.* to 'b101'@'localhost';

use onedb;
