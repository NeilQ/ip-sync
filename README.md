# ip-sync
###简介
这是一个利用dropbox, 快盘等云盘同步远程主机的公网ip的小工具。
运行后，它将监控公网ip, 并将最新的ip地址写入dropbox文件夹（或者其他文件夹）, 这样, 你在任何地方都可以通过dropbox知道这台电脑的公网ip。

假如你有一台需要远程连接的主机，或者你有一个树莓派，可以将其加壳设为开机启动，就无需担心公网ip变化了。
![pic](./1.png)

###使用说明
```
ipsync -f /f/dropbox -d 60
``` 
**-f** dropbox文件夹物理地址

**-d** ip监控延迟时间，秒为单位

###安装
```
python setup.py install
```


###Profile
This is a tool to sync public ip of a pc, which using dropbox or any other cloud storage.
When it runs, it will monitor the public ip address and write to dropbox directory. In this way, you can always know the public ip of the pc.

I thinks it can be set to run on starup so that you will never worry about the ip changes or the pc reboots, if you have a remote pc or a Raspberry Pi.
![pic](./1.png)

###How to use?
```
ipsync -f /f/dropbox -d 60
``` 
**-f** The physic path of dropbox folder.

**-d** The delay time of ip monitor(seconds). 

###Install
```
python setup.py install
```
