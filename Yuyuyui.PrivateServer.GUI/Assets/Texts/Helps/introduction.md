# What Is This Project?

This project is a private server for the mobile game Yuyuyui.
It serves as a preservation of the game content after the game shuts down.
It also comes with an Account Transfer tool that allows you to transfer
your account from the official server to this private server. It supports
both Android and iOS clients.

This project is actively in development. However, new in-game features may
or may not come due to the game shutdown.

## How Does It Work?

The private server is based on
[Man-in-the-middle attack](https://en.wikipedia.org/wiki/Man-in-the-middle_attack)
(MITM in short). This technique is used for intercepting the messages from
the game client and creating responses that simulate the behavior of the
original official server.

When you start the private server or the account transfer tool, a proxy
that handles the requests and responses will be started. You are required
to connect your mobile device to the proxy server, download a certificate
that is generated randomly by this program (since the game uses HTTPS for
communication), and trust it manually. After that, your game will be able
to get the responses from the private server, instead of the official server.

## Is It Safe?

Daunting as it may sound, it is completely safe, as long as you don't share
the generated certificate with others. Each certificate is generated
randomly, which means that your certificate is different from any other
user, or any other program that utils MITM attack.

The program will only intercept those requests used for running the game.
The program will **not** record any data other than your game account.
However, due to the nature of this program, you have to make sure that you
downloaded it from a trusted source.

***NOTE: Using this program means that you fully understand the risk. The***
***author(s) of this program does not take any responsibility in case of***
***any data lost, leaked, or any other event.***
