# ChronoFag Timer

![DotBadge](http://rebornix.qiniudn.com/dotbadge.svg) [![Github Downloads](https://img.shields.io/github/downloads/HarpyWar/chronofag-timer/total.svg?maxAge=2592000)](https://github.com/HarpyWar/chronofag-timer/releases)

This is a productivity timer inspired by a [pomodoro technique](https://en.wikipedia.org/wiki/Pomodoro_Technique).

It aims to help you save a time and increase a productivity if most of the day you spend at the computer.


By default there is a classical pomodoro time of 25 minutes and break of 5 minutes. After first 4 pomodoros there is a break of 15 minutes and after 8 pomodoros &mdash; a break of 30 minutes (these settings can be changed in `config.hjson`).

![Pomodoro Timer](http://i.imgur.com/xfjKEBF.png)

This timer differs from similar others by fully automation and locking a screen when a break. So you don't need manually click start or regulate a time.

Sometimes you may need a computer urgently when timer show a break. There is an extra time button that allows to add several minutes to work timer, and immediately switch to work mode. 

![Break Timer](http://i.imgur.com/wkcOFDM.png)

*Lock mode can be enabled for a basic protection from yourself (or childrens if you want to limit time for them). With enabled `lockmode` you will not able to close the program with any way, except kill a process in task manager.

For better protect you can disable Task Manager in Windows policy and remove write permissions for `config.hjson`. Timer restores a previous session after the program restart, so a computer reboot will not help to reset a break.*


It also have automatically adjustment for timers when you AFK for a while.

In additional it contains a feature to add separate single timers. 

For example someone ask you to do something exactly after a hour, or you need remember to turn off eggs after 10 minutes. 
You will always see when a timer is elapsed, so you can't miss important things anymore.

![Custom User Timers](http://i.imgur.com/EB2czqP.png)

## All Features

* Flexible settings for work and break timers
* Adjustment of time after AFK (including a computer sleep mode)
* Show timer notifications for predefined time before it elapsed
* Restore timer after program restart
* Stop/start timer from menu if you want to temporary disable timer, without full exit
* Autostart with Windows (optional)
* Sound notifications
* Lock controls during break time (option `lockmode`). 
* Extra time button (option `max_extratimes`). 
* Add single user timers
* Customizable color style


## Donation

If this timer was valuable to you, you can support development by donating
* [Paypal](https://www.paypal.me/harpywar)
* Yandex.Money `4100176229161`
* Webmoney  `R306333557133` `Z170879613351`
* Bitcoin `1855gBwvXdV9XfiCVzTgY323wSp9boJLKm`
