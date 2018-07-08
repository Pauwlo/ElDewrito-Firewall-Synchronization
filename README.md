# ElDewrito Firewall Synchronization

A Windows service to synchronize the Windows firewall with a MySQL database containing bans.

## Requirements

To run this application, you'll need:

* Windows 7 or later
* .NET Framework 4.5.2 or later
* A MySQL database

## Getting started

### Installation

1. Create your MySQL table by running the following SQL script on your database:

```
CREATE TABLE IF NOT EXISTS `bans` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `ip` varchar(32) NOT NULL,
    `date` datetime NOT NULL,
    `description` varchar(255) NOT NULL,
    `username` varchar(32) NOT NULL,
    `uid` varchar(16) NOT NULL,

    PRIMARY KEY (`id`),
    UNIQUE KEY `ip` (`ip`),
    UNIQUE KEY `date` (`date`)
);
```

*Note: You can change the table name `bans` by anything else.*

2. Extract the following files somewhere on your machine(s):

- ElDewritoFirewallSynchronization.exe
- MySql.Data.dll
- Settings.xml

3. Start a Command Prompt/PowerShell with administrator privileges, and run the following command:

```
sc create "ElDewrito Firewall Synchronization" binPath= "PATH_TO_THE_EXE" start= auto
```

*Note 1: Of course, replace "PATH_TO_THE_EXE" by an absolute file path, leading to the application executable. For example: `binPath= "C:\Program Files (x86)\ElDewrito Firewall Synchronization\ElDewritoFirewallSynchronization.exe"`*

*Note 2: Yes, there is a space after the `=` signs. Don't ask me why.*

4. Open the configuration file `Settings.xml` and enter your database details.

```
hostname: The MySQL database hostname. (default: localhost)
database: The database name. (default: eldewrito)
table: The table name. (default: bans)
username: The database username to log in. This app only needs the SELECT permission. (default: root)
password: The database password for the specified user. (default: (empty))
ssl: If your database supports SSL, enable this. (default: false)
```

5. Start the service with the following command:

```
sc start "ElDewrito Firewall Synchronization"
```

*Note: The service will be started automatically at boot. You only need to start it manually now, or restart your computer.*

### Usage

Now that the service is installed and running, you can add bans in your database. The service will synchronize every 20 seconds and add a firewall rule for each new ban.

It will also remove the rules if you delete them from the database.

You can ban IPs and subnets (by using the CIDR format). Only "ip" and "date" fields are required.

## Troubleshooting & Contributing

This application uses Windows Events to log warnings and errors.

If something is not working properly, please open the Event Viewer (in Windows Administrative Tools) and look for `ElDewrito Firewall Synchronization` events in `Windows Logs\Application`.

You can also contact me on Discord: `Pauwlo#0117`

**Enjoy!**
